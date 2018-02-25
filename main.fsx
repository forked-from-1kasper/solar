#r "solar.dll"

open System.Windows.Forms
open System.Drawing
open System

open Solar
open Solar.Improvements
open Solar.Constants
open Solar.Physics
open Solar.Graphics

let earth =
    { speed = { x = 0.0; y = 29783.0 };
      acc = { x = 0.0; y = 0.0 };
      pos = { x = 147098290000.0; y = 0.0 };
      m = 5.9726 * (10.0 ** 24.0);
      id = "Earth";

      visibleRadius = 3.0;
      visibleColor = Color.LightBlue }

let venus =
    { speed = { x = 0.0; y = -35020.0 };
      acc = { x = 0.0; y = 0.0 };
      pos = { x = 107476259000.0; y = 0.0 };
      m = 4.8675 * (10.0 ** 24.0);
      id = "Venus";

      visibleRadius = 2.8;
      visibleColor = Color.Orange }

let mercury =
    { speed = { x = 0.0; y = 47360.0 };
      acc = { x = 0.0; y = 0.0 };
      pos = { x = 46001009000.0; y = 0.0 };
      m = 3.33022 * (10.0 ** 23.0);
      id = "Mercury";

      visibleRadius = 1.0;
      visibleColor = Color.Brown }

let mars =
    { speed = { x = 0.0; y = 24130.0 };
      acc = { x = 0.0; y = 0.0 };
      pos = { x = 206655000000.0; y = 0.0 };
      m = 6.4171 * (10.0 ** 23.0);
      id = "Mars";

      visibleRadius = 1.8;
      visibleColor = Color.Red }

let sun =
    { speed = { x = 0.0; y = 0.0 };
      acc = { x = 0.0; y = 0.0 };
      pos = { x = 0.0; y = 0.0 };
      m = 1.9885 * (10.0 ** 30.0);
      id = "Sun";

      visibleRadius = 10.0;
      visibleColor = Color.Yellow }

let mutable bodies : Body list = [ sun; mercury; venus; earth; mars ]

let form = new Form ()

let size = Size (width, height)
form.Size <- size
form.MinimumSize <- size
form.MaximumSize <- size

form.Text <- "Earth disappears"

let g = form.CreateGraphics ()
g.SmoothingMode <- Drawing2D.SmoothingMode.AntiAlias

let bitmap = new Bitmap (width, height)
let second_buffer = Graphics.FromImage bitmap
second_buffer.SmoothingMode <- Drawing2D.SmoothingMode.AntiAlias

let timer = new Timer ()
timer.Interval <- int $ interval * 1000.0

second_buffer.Clear Color.Black

let printParameters (bodies : Body list) =
    List.map
        (fun here ->
            sprintf "pos %s speed %s acc %s id %s" (string here.pos)
                                                   (string here.speed)
                                                   (string here.acc)
                                                   here.id)
        bodies

let font = new Font ("Consolas", 8.0f)
let brush = new SolidBrush (Color.White)

let settingsForm = new Form ()
settingsForm.MinimumSize <- new Size (100, 70)
settingsForm.MaximumSize <- new Size (1000, 70)
settingsForm.Text <- "Speed control"

let speedBar = new TrackBar ()
//speedBar.Size <- new Size (0, 0)
speedBar.Dock <- DockStyle.Fill
speedBar.Location <- new Point (0, 0)
speedBar.Maximum <- 10000
speedBar.Text <- "goggog"

settingsForm.Controls.Add(speedBar)
settingsForm.Show ()

let tick _ =
    let dt = 24.0 * 60.0 * 60.0 * interval * (float speedBar.Value)
    
    second_buffer.Clear Color.Black

    List.iter (drawBody second_buffer) bodies
    bodies <- List.map (updateBody bodies dt) bodies

    List.iteri
        (fun index text ->
            let rect = new RectangleF (0.0f, float32 index * 20.0f, 0.0f, 0.0f)
            second_buffer.DrawString (text, font, brush, rect))
        (printParameters bodies)

    g.DrawImageUnscaled(bitmap, 0, 0)
    ()

timer.Tick.Add tick
timer.Start ()

Application.Run form

timer.Stop ()

timer.Dispose ()
form.Dispose ()
bitmap.Dispose ()

font.Dispose ()
brush.Dispose ()
speedBar.Dispose ()
