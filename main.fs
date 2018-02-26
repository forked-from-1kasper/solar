open System.Windows.Forms
open System.Drawing
open System

open Solar
open Solar.Improvements
open Solar.Constants
open Solar.Physics
open Solar.Graphics
open Solar.Xml

let mutable bodies : Body list = []

let printParameters (bodies : Body list) =
    List.map
        (fun here ->
            sprintf "pos %s speed %s acc %s id %s" (string here.pos)
                                                   (string here.speed)
                                                   (string here.acc)
                                                   here.id)
        bodies

let settingsForm = new Form ()
settingsForm.MinimumSize <- new Size (100, 70)
settingsForm.MaximumSize <- new Size (1000, 70)
settingsForm.Text <- "Speed control"

let speedBar = new TrackBar ()
speedBar.Dock <- DockStyle.Fill
speedBar.Location <- new Point (0, 0)
speedBar.Maximum <- 1000
speedBar.Text <- "goggog"

settingsForm.Controls.Add(speedBar)

let tick (first : Graphics) (second : Graphics) (secondBitmap : Bitmap) (font : Font) (brush : Brush) _ =
    let dt = 24.0 * 60.0 * 60.0 * interval * (float speedBar.Value)
    
    second.Clear Color.Black

    List.iter (drawBody second) bodies
    bodies <- List.map (updateBody bodies dt) bodies

    List.iteri
        (fun index text ->
            let rect = new RectangleF (0.0f, float32 index * 20.0f, 0.0f, 0.0f)
            second.DrawString (text, font, brush, rect))
        (printParameters bodies)

    first.DrawImageUnscaled(secondBitmap, 0, 0)
    ()

[<EntryPoint>]
let main argv =
    if argv.Length >= 1 then
        bodies <- parseFile <| argv.[0]
        
        use form = new Form ()

        let size = Size (width, height)
        form.Size <- size
        form.MinimumSize <- size
        form.MaximumSize <- size

        form.Text <- "Earth disappears"

        let g = form.CreateGraphics ()
        g.SmoothingMode <- Drawing2D.SmoothingMode.AntiAlias

        use bitmap = new Bitmap (width, height)
        let second_buffer = Graphics.FromImage bitmap
        second_buffer.SmoothingMode <- Drawing2D.SmoothingMode.AntiAlias

        use timer = new Timer ()
        timer.Interval <- int $ interval * 1000.0

        second_buffer.Clear Color.Black

        use font = new Font ("Consolas", 8.0f)
        use brush = new SolidBrush (Color.White)

        timer.Tick.Add (tick g second_buffer bitmap font brush)

        timer.Start ()

        settingsForm.Show ()
        Application.Run form

        timer.Stop ()

        speedBar.Dispose ()
        
        0
    else
        printfn "error: missing arguments."
        -1
