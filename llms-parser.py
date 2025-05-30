#!/usr/bin/env python3
"""
Website Content Crawler

A Python script that crawls websites and extracts text content, converting HTML to markdown format.
Specifically designed to crawl VisioForge website content while excluding help pages.

Author: VisioForge
License: CC-BY-4.0 (https://creativecommons.org/licenses/by/4.0/)

Features:
- Web crawling with automatic page discovery
- HTML to markdown conversion
- Duplicate URL detection and filtering
- Content filtering (excludes /help/ pages)
- Respectful crawling with delays and proper headers
"""

import os
import re
import requests
from bs4 import BeautifulSoup
from urllib.parse import urljoin, urlparse
import time
from collections import deque
from markdownify import markdownify as md

def normalize_url(url):
    """Normalize URL by removing fragments, standardizing protocol, and handling trailing slashes"""
    parsed = urlparse(url)
    
    # Always use https for consistency
    scheme = "https"
    
    # Handle path normalization
    path = parsed.path
    # Convert empty path to "/"
    if not path:
        path = "/"
    # Remove trailing slash for non-root paths
    elif path != "/" and path.endswith("/"):
        path = path[:-1]
    
    # Reconstruct normalized URL
    normalized = f"{scheme}://{parsed.netloc}{path}"
    if parsed.query:
        normalized += f"?{parsed.query}"
    
    return normalized

def is_valid_url(url):
    """Check if URL should be crawled"""
    parsed = urlparse(url)
    # Only crawl pages from the same domain
    if parsed.netloc != 'www.visioforge.com':
        return False
    # Exclude /help/ pages
    if '/help/' in parsed.path:
        return False
    return True

def extract_and_convert_to_markdown(html_content):
    """Extract content from HTML and convert to markdown"""
    soup = BeautifulSoup(html_content, 'html.parser')
    
    # Remove script, style, and other non-content elements
    for script in soup(["script", "style", "nav", "footer", "header"]):
        script.decompose()
    
    # Find the main content area (try common content selectors)
    main_content = soup.find('main') or soup.find('article') or soup.find(class_=re.compile(r'content|main|article', re.I)) or soup.find('body')
    
    if main_content:
        # Convert the main content to markdown
        markdown_content = md(str(main_content), 
                            heading_style="ATX",  # Use # style headings
                            bullets="-"  # Use - for bullet points
                           )
    else:
        # Fallback to full body conversion
        markdown_content = md(str(soup), 
                            heading_style="ATX",
                            bullets="-"
                           )
    
    # Clean up excessive whitespace and normalize
    markdown_content = re.sub(r'\n\s*\n\s*\n', '\n\n', markdown_content)  # Remove multiple empty lines
    markdown_content = re.sub(r'[ \t]+', ' ', markdown_content)  # Normalize spaces
    return markdown_content.strip()

def get_links_from_page(html_content, current_url):
    """Extract all links from the current page"""
    soup = BeautifulSoup(html_content, 'html.parser')
    links = []
    
    for link in soup.find_all('a', href=True):
        href = link['href']
        # Convert relative URLs to absolute
        absolute_url = urljoin(current_url, href)
        if is_valid_url(absolute_url):
            links.append(absolute_url)
    
    return links

def write_file_header(outfile):
    """
    Write header with author and license information to the output file
    """
    header = """# VisioForge Documentation and Website Content

**Author:** VisioForge  
**License:** CC-BY-4.0 (https://creativecommons.org/licenses/by/4.0/)  
**Generated:** This file contains comprehensive documentation and website content from VisioForge  
**Content Sources:** Markdown documentation files and website pages from www.visioforge.com  

This work is licensed under the Creative Commons Attribution 4.0 International License.

---

"""
    outfile.write(header)

def process_local_markdown_files(outfile, separator):
    """
    Process all local .md files in the current directory and subdirectories
    """
    print("Processing local markdown files...")
    file_count = 0
    
    for root, _, files in os.walk("."):
        for filename in files:
            if filename.endswith(".md"):
                filepath = os.path.join(root, filename)
                try:
                    with open(filepath, "r", encoding="utf-8") as infile:
                        content = infile.read()
                        
                        # Write to file
                        outfile.write(f"# Local File: {filepath}\n\n")
                        outfile.write(content)
                        outfile.write(separator)
                        
                        file_count += 1
                        print(f"Successfully processed local file {file_count}: {filepath}")
                        
                except Exception as e:
                    print(f"Error processing local file {filepath}: {e}")
                    continue
    
    print(f"Processed {file_count} local markdown files")
    return file_count

def create_llms_full_text():
    """
    Processes local markdown files and crawls pages from http://www.visioforge.com (excluding /help/ pages),
    extracts text content, converts to markdown, and concatenates into a single llms-full.txt file.
    A separator is added between the content of each page/file.
    """
    base_url = "http://www.visioforge.com"
    output_filename = "llms-full.txt"
    separator = "\n---END OF PAGE---\n\n"
    
    # Set to keep track of visited URLs
    visited_urls = set()
    # Queue for URLs to visit
    urls_to_visit = deque([base_url])
    
    # Session for connection pooling
    session = requests.Session()
    session.headers.update({
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'
    })
    
    # Initialize counters
    local_file_count = 0
    web_page_count = 0
    
    with open(output_filename, "w", encoding="utf-8") as outfile:
        # Write file header with license and author information
        write_file_header(outfile)
        
        # First, process all local markdown files
        local_file_count = process_local_markdown_files(outfile, separator)
        
        # Then, crawl web pages
        print("\nStarting web crawling...")
        while urls_to_visit and web_page_count < 5000:  # Increased limit for full crawling
            current_url = urls_to_visit.popleft()
            
            # Normalize URL for duplicate checking
            normalized_url = normalize_url(current_url)
            
            if normalized_url in visited_urls:
                continue
                
            visited_urls.add(normalized_url)
            
            try:
                print(f"Processing: {current_url}")
                response = session.get(current_url, timeout=20)
                response.raise_for_status()
                
                # Extract and convert content to markdown
                markdown_content = extract_and_convert_to_markdown(response.text)
                
                # Write to file (use original URL for display)
                outfile.write(f"# Page: {current_url}\n\n")
                outfile.write(markdown_content)
                outfile.write(separator)
                
                # Get links from this page
                links = get_links_from_page(response.text, current_url)
                for link in links:
                    normalized_link = normalize_url(link)
                    if normalized_link not in visited_urls:
                        urls_to_visit.append(link)
                
                web_page_count += 1
                print(f"Successfully processed page {web_page_count}: {current_url}")
                
                # Be polite - add small delay between requests
                time.sleep(1)
                
            except Exception as e:
                print(f"Error processing {current_url}: {e}")
                continue
    
    print(f"Finished creating {output_filename}")
    print(f"- Local markdown files: {local_file_count}")
    print(f"- Web pages crawled: {web_page_count}")
    print(f"- Total content sources: {local_file_count + web_page_count}")

if __name__ == "__main__":
    create_llms_full_text() 