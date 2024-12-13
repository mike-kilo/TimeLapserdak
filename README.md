My first serious project in AvaloniaUI - a GUI for creating timelapses and hyperlapses from a series of pictures stored in given folder.

Current UI looks as this:
![TimeLapserdak_locks](https://github.com/user-attachments/assets/00a7bb46-bf15-45c3-bd94-6543372e4e9e)

You are presented with two pictures - the first one and the last one from the series stored in the selected folder.
With the cropping frames you can select the starting and ending frame (cropping).
When pressed on "Generate", a hyper-lapse video is generated.

Current status is:
  - UI is preliminarily defined
  - it is possible to navigate to the folder and load the pictures
  - it is possible to select the cropping areas of both starting and ending images
  - the software generates intermediate (cropped and scaled to 1920x1080) images

TODO:
  - generate video from the intermediate pictures

Limitations
  - only 1920x1080 output resolution is currently supported
  - the starting and ending images cropping areas size and position are presented in device pixels (desired: image pixels)
  - slight aspect ratio inaccuracy appears as cropping area size is continuous and not stepped (16 pixels for width, 9 pixels for height)

Any comments and suggestions are welcome.
