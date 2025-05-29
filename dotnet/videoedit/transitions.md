---
title: Video Transitions and SMPTE Wipe Effects for .NET
description: Explore the full range of video transition effects available in Video Edit SDK for .NET, including over 100 SMPTE standard wipes with customizable properties like border color, softness, width, and positioning. Learn how to implement professional-grade transitions in your .NET video applications.
sidebar_label: Transitions

---

# Transitions

[!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net)

The SMPTE Wipe transition produces any of the standard wipes defined by the Society of Motion Picture and Television Engineers (SMPTE) in SMPTE document 258M-1993, except for the quad split.

## Properties

| Property        | Type          | Default | Description |
|-----------------|---------------|---------|-------------|
| Border Color    | Color/TColor | Black   | Color of the border around the edges of the wipe pattern. |
| BorderSoftness  | long          | 0       | Width of the blurry region around the edges of the wipe pattern. Specify zero for no blurry region. |
| BorderWidth     | long          | 0       | Width of the solid border along the edges of the wipe pattern. Specify zero for no border. |
| ID              | long          | 1       | Standard SMPTE wipe code specifying the style of wipe to use. For a list of wipe codes and their associated schematics, see SMPTE document 258M-1993. |
| OffsetX         | long          | 0       | Horizontal offset of the wipe origin from the center of the image. Valid only for values of **ID** from 101 to 131. |
| OffsetY         | long          | 0       | Verical offset of the wipe origin from the center of the image. Valid only for values of **ID** from 101 to 131. |
| ReplicateX      | long          | 0       | Number of times to replicate the wipe pattern horizontally. Valid only for values of **ID** from 101 to 131. |
| ReplicateY      | long          | 0       | Number of times to replicate the wipe pattern vertically. Valid only for values of **ID** from 101 to 131. |
| ScaleX          | double        | 1.0     | Amount by which to stretch the wipe horizontally, as a percentage of the original wipe definition. Valid only for values of **ID** from 101 to 131. |
| ScaleY          | double        | 1.0     | Amount by which to stretch the wipe vertically, as a percentage of the original wipe definition. Valid only for values of **ID** from 101 to 131. |

## SMPTE

This transition supports the following standard SMPTE wipes:

| Number | Description                       | Number | Description                                |
|--------|-----------------------------------|--------|--------------------------------------------|
| 1      | Horizontal                        | 211    | Radial, left-right, top                    |
| 2      | Vertical                          | 212    | Radial, up-down, right                     |
| 3      | Upper left                        | 213    | Radial, left-right, top-bottom             |
| 4      | Upper right                       | 214    | Radial, up-down, left-right                |
| 5      | Lower right                       | 221    | Radial, top                                |
| 6      | Lower left                        | 222    | Radial, right                              |
| 7      | Four corners                      | 223    | Radial, bottom                             |
| 8      | Four squares                      | 224    | Radial, left                               |
| 21     | Barn doors, vertical              | 225    | Radial, top clockwise, bottom clockwise    |
| 22     | Barn doors, horizontal            | 226    | Radial, left clockwise, right clockwise    |
| 23     | Top center                        | 227    | Radial, top clockwise, bottom anticlockwise |
| 24     | Right center                      | 228    | Radial, left clockwise, right anticlockwise |
| 25     | Bottom center                     | 231    | Radial, top split                          |
| 26     | Left center                       | 232    | Radial, right split                        |
| 41     | Diagonal, NW to SE                | 233    | Radial, bottom split                       |
| 42     | Diagonal, NE to SW                | 234    | Radial, left split                         |
| 43     | Triangles, top/bottom             | 235    | Radial, top-bottom split                   |
| 44     | Triangles, left/right             | 236    | Radial, left-right split                   |
| 45     | Diagonal stripe, SW to NE         | 241    | Radial, top-left corner                    |
| 46     | Diagonal stripe, NW to SE         | 242    | Radial, bottom-left corner                 |
| 47     | Cross                             | 243    | Radial, bottom-right corner                |
| 48     | Diamond Box                       | 244    | Radial, top-right corner                   |
| 61     | Wedge, top                        | 245    | Radial, top-left, bottom-right             |
| 62     | Wedge, right                      | 246    | Radial, bottom-left, top-right             |
| 63     | Wedge, bottom                     | 251    | Center radial, top                         |
| 64     | Wedge, left                       | 252    | Center radial, left                        |
| 65     | V                                 | 253    | Center radial, bottom                      |
| 66     | V, right                          | 254    | Center radial, right                       |
| 67     | V, inverted                       | 261    | Box radial, right                          |
| 68     | V, left                           | 262    | Box radial, top                            |
| 71     | Sawtooth, left                    | 263    | Center radial, top, bottom                 |
| 72     | Sawtooth, top                     | 264    | Center radial, left, right                 |
| 73     | Sawtooth, vertical                | 301    | Matrix, horizontal                         |
| 74     | Sawtooth, horizontal              | 302    | Matrix, vertical                           |
| 101    | Box                               | 303    | Matrix, diagonal, top-left                 |
| 102    | Diamond                           | 304    | Matrix, diagonal, top-right                |
| 103    | Triangle, up                      | 305    | Matrix, diagonal, bottom-right             |
| 104    | Triangle, right                   | 306    | Matrix, diagonal, bottom-left              |
| 105    | Triangle, bottom                  | 310    | Matrix, clockwise top-left                 |
| 106    | Triangle, left                    | 311    | Matrix, clockwise top-right                |
| 107    | Arrow head, up                    | 312    | Matrix, clockwise bottom-right             |
| 108    | Arrow head, right                 | 313    | Matrix, clockwise bottom-left              |
| 109    | Arrow head, down                  | 314    | Matrix, anticlockwise top-left             |
| 110    | Arrow head, left                  | 315    | Matrix, anticlockwise top-right            |
| 111    | Pentagon, up                      | 316    | Matrix, anticlockwise bottom-right         |
| 112    | Pentagon, down                    | 317    | Matrix, anticlockwise bottom-left          |
| 113    | Hexagon                           | 320    | Matrix, vertical top-left, top-right       |
| 114    | Hexagon, rotated                  | 321    | Matrix, vertical bottom-left, bottom-right |
| 119    | Circle                            | 322    | Matrix, vertical top-left, bottom-right    |
| 120    | Oval, horizontal                  | 323    | Matrix, vertical bottom-left, top-right    |
| 121    | Oval, vertical                    | 324    | Matrix, horizontal top-left, bottom-left   |
| 122    | Eye, horizontal                   | 325    | Matrix, horizontal top-right, bottom-right |
| 123    | Eye, vertical                     | 326    | Matrix, horizontal top-left, bottom-right  |
| 124    | Rounded rectangle, horizontal     | 327    | Matrix, horizontal top-right, bottom-left  |
| 125    | Rounded rectangle, vertical       | 328    | Matrix, diagonal bottom-left, top-right    |
| 127    | 4-point star                      | 329    | Matrix, diagonal top-left, bottom-right    |
| 128    | 4-point star                      | 340    | Matrix, top double spiral                  |
| 129    | 6-point star                      | 341    | Matrix, bottom double spiral               |
| 130    | Heart                             | 342    | Matrix, left double spiral                 |
| 131    | Keyhole                           | 343    | Matrix, right double spiral                |
| 201    | Radial, 12 o'clock                | 344    | Matrix, quad spiral, top-bottom            |
| 202    | Radial, 3 o'clock                 | 345    | Matrix, quad spiral, left-right            |
| 203    | Radial, 6 o'clock                 | 350    | Waterfall, left                            |
| 204    | Radial, 9 o'clock                 | 351    | Waterfall, right                           |
| 205    | Radial, 12 + 6 o'clock            | 352    | Waterfall, horizontal, left                |
| 206    | Radial, 3 + 9 o'clock             | 353    | Waterfall, horizontal, right               |
| 207    | Radial, 4-way                     | 409    | Random mask                                |
  
---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
