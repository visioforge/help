import os
import re # Added re module for regex operations

def create_llms_full_text():
    """
    Parses all .md files in the current directory and its subdirectories,
    and concatenates their content into a single llms-full.txt file.
    A separator is added between the content of each file.
    Removes YAML frontmatter from the beginning of .md files.
    """
    output_filename = "llms-full.txt"
    separator = "\n---END OF FILE---\n\n"  # Added newlines for better readability

    # Regex to match YAML frontmatter (lines between --- at the start of the file)
    frontmatter_pattern = re.compile(r"^---\s*\n(.*?\n)*?^---\s*\n", re.MULTILINE | re.DOTALL)

    with open(output_filename, "w", encoding="utf-8") as outfile:
        for root, _, files in os.walk("."):  # Iterate through current dir and subdirs
            for filename in files:
                if filename.endswith(".md"):
                    filepath = os.path.join(root, filename)
                    try:
                        with open(filepath, "r", encoding="utf-8") as infile:
                            content = infile.read()
                            # Remove frontmatter
                            content_without_frontmatter = frontmatter_pattern.sub("", content, count=1)
                            outfile.write(content_without_frontmatter.strip()) # Also strip leading/trailing whitespace
                            outfile.write(separator)
                        print(f"Successfully processed: {filepath}")
                    except Exception as e:
                        print(f"Error processing file {filepath}: {e}")

    print(f"Finished creating {output_filename}")

if __name__ == "__main__":
    create_llms_full_text() 