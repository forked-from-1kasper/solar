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

[<EntryPoint>]
let main argv =
    let printParameters (bodies : Body list) =
        List.map
            (fun here ->
                sprintf "pos %s speed %s acc %s id %s"
                    (string here.pos)
                    (string here.speed)
                    (string here.acc)
                    here.id)
            bodies
    
    let mutable current_planet = 0
    let moveToPlanet n =
        x_offset <-
            (bodies.[n].pos.x / -space_scale)
        y_offset <-
            (bodies.[n].pos.y / -space_scale)
    
    let keypressEvent (bodies : Body list) (e : KeyPressEventArgs) =
        match e.KeyChar with
        | 's' | 'S' -> y_offset <- y_offset - move_offset
        | 'w' | 'W' -> y_offset <- y_offset + move_offset
        | 'd' | 'D' -> x_offset <- x_offset - move_offset
        | 'a' | 'A' -> x_offset <- x_offset + move_offset
        | 'q' | 'Q' -> z_offset <- z_offset - move_offset
        | 'e' | 'E' -> z_offset <- z_offset + move_offset        

        | '+' -> dt_scale <- dt_scale * dt_scale_offset
        | '-' -> dt_scale <- dt_scale / dt_scale_offset

        | '.' -> space_scale <- space_scale * space_scale_offset
        | ',' -> space_scale <- space_scale / space_scale_offset

        | '0' ->
            if current_planet + 1 = bodies.Length then ()
            else current_planet <- current_planet + 1
        | '9' ->
            if current_planet = 0 then ()
            else current_planet <- current_planet - 1
        | '8' ->
            moveToPlanet current_planet

        | 'p' | 'P' ->
            currentProjection <-
                match currentProjection with
                | XY -> XZ
                | XZ -> YZ
                | YZ -> XY

        | _ -> ()

    let tick (first : Graphics)
             (second : Graphics)
             (secondBitmap : Bitmap)
             (font : Font) (brush : Brush) _ =
        let dt = 24.0 * 60.0 * 60.0 * interval * dt_scale
        bodies <- List.map (updateBody bodies dt) bodies
    
        second.Clear Color.Black

        List.iter (drawBody second) bodies

        let rect = new RectangleF (0.0f, 0.0f, 0.0f, 0.0f)
        let playerX = x_offset * -space_scale
        let playerY = y_offset * -space_scale
        let playerZ = z_offset * -space_scale
        let selectedPlanetId = bodies.[current_planet].id
        let projection = currentProjection.ToString ()
        let text =
            sprintf "player position {%f, %f, %f} scale %f time_scale %f planet %s projection %s"
                playerX playerY playerZ
                space_scale dt_scale
                selectedPlanetId
                projection
        second.DrawString (text, font, brush, rect)
        List.iteri
            (fun index text ->
                let rect = new RectangleF (0.0f, float32 (index + 1) * 20.0f, 0.0f, 0.0f)
                second.DrawString (text, font, brush, rect))
            (printParameters bodies)

        first.DrawImageUnscaled(secondBitmap, 0, 0)

        ()
    
    let configFileName =
        if argv.Length >= 1 then argv.[0]
        else "solar_system.xml"
    bodies <- parseFile configFileName
        
    use form = new Form ()

    let size = Size (width, height)
    form.Size <- size
    form.MinimumSize <- size
    form.MaximumSize <- size

    form.Text <- "Earth disappears"

    form.KeyPress.Add (keypressEvent bodies)

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

    Application.Run form

    timer.Stop ()        
    0
