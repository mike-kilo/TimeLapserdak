My first serious project in AvaloniaUI - a GUI for creating timelapses and hyperlapses from a series of pictures stored in given folder.

The name is pun on "Time-Lapse" (a technique in which the frequency at which film frames are captured (the frame rate) is much lower than the frequency used to view the sequence) and a Polish word "Łapserdak" (rouge, scoundrel).

The software takes a series of JPEG images (all stored in one folder) and converts them to a time-lapse video. It is possible to crop the starting frame and the ending frame, thus creating a sort of hyper lapse.

I came up with an idea of this software when I was struggling with generating time-lapse videos taken as a series of pictures with my GoPro Hero Session 4 action camera (I used commandline to crop/scale the images and then encode them with FFMpeg). And at the same time I wanted to gain some experience with AvaloniaUI.

Current UI looks as this:
![TimeLapserdak_forest](https://github.com/user-attachments/assets/b2c3f90e-761f-47dc-9dc5-7c48ee16684f)


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
