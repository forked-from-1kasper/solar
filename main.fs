open System.Windows.Forms
open System.Drawing
open System

open Solar
open Solar.Improvements
open Solar.Constants
open Solar.Physics
open Solar.Graphics
open Solar.Xml
open Solar.Keymap

let mutable bodies : Body list = []

[<EntryPoint>]
let main argv =    
    let mutable current_planet = 0

    let mutable paused = false
    let mutable showInfo = true
    let mutable showOrbits = false
    let mutable timePassed = 0.0

    use form = new Form ()
    
    let size = Size (width, height)
    form.Size <- size
    form.MinimumSize <- size
    form.MaximumSize <- size

    form.Text <- "Earth disappears"

    let firstBuffer = form.CreateGraphics ()
    firstBuffer.SmoothingMode <- Drawing2D.SmoothingMode.AntiAlias

    use secondBitmap = new Bitmap (Constants.width, Constants.height)
    let secondBuffer = Graphics.FromImage secondBitmap
    secondBuffer.SmoothingMode <- Drawing2D.SmoothingMode.AntiAlias

    use orbitsBitmap = new Bitmap (Constants.width, Constants.height)
    let orbitsBuffer = Graphics.FromImage orbitsBitmap

    let printParameters (bodies : Body list) =
        List.map
            (fun here ->
                sprintf "pos %s speed %s acc %s id %s"
                    (string here.pos)
                    (string here.speed)
                    (string here.acc)
                    here.id)
            bodies
    
    let moveToPlanet n =
        x_offset <-
            (bodies.[n].pos.x / -space_scale)
        y_offset <-
            (bodies.[n].pos.y / -space_scale)
    
    let keypressEvent (bodies : Body list) (e : KeyPressEventArgs) =
        let c = Char.ToLower e.KeyChar
        if isMoving c then orbitsBuffer.Clear Color.Transparent
        match c with
        | DOWN -> y_offset <- y_offset - move_offset
        | UP -> y_offset <- y_offset + move_offset
        | RIGHT -> x_offset <- x_offset - move_offset
        | LEFT -> x_offset <- x_offset + move_offset
        | Z_DOWN -> z_offset <- z_offset - move_offset
        | Z_UP -> z_offset <- z_offset + move_offset        

        | SCALE_UP -> space_scale <- space_scale * space_scale_offset
        | SCALE_DOWN -> space_scale <- space_scale / space_scale_offset

        | PLANET_NEXT ->
            if current_planet + 1 = bodies.Length then ()
            else current_planet <- current_planet + 1
        | PLANET_BACK ->
            if current_planet = 0 then ()
            else current_planet <- current_planet - 1
        | PLANET_GO ->
            moveToPlanet current_planet

        | CHANGE_PROJECTION ->
            currentProjection <-
                match currentProjection with
                | XY -> XZ
                | XZ -> YZ
                | YZ -> XY

        | DT_SCALE_UP -> dt_scale <- dt_scale * dt_scale_offset
        | DT_SCALE_DOWN -> dt_scale <- dt_scale / dt_scale_offset
        | PAUSE -> paused <- not paused

        | TOGGLE_INFO -> showInfo <- not showInfo
        | TOGGLE_ORBITS -> showOrbits <- not showOrbits
        | _ -> ()

    let tick (font : Font) (brush : Brush) _ =
        let dt = 24.0 * 60.0 * 60.0 * interval * dt_scale

        if not paused then
            bodies <- List.map (updateBody bodies dt) bodies
            timePassed <- timePassed + dt

        secondBuffer.Clear Color.Black
        if not showOrbits then orbitsBuffer.Clear Color.Transparent

        List.iter
            (fun b ->
                drawBody secondBuffer b
                drawBodyTag secondBuffer b)
            bodies
        List.iter (drawBodyPoint orbitsBuffer) bodies

        let rect = new RectangleF (0.0f, 0.0f, 0.0f, 0.0f)
        let playerX = x_offset * -space_scale
        let playerY = y_offset * -space_scale
        let playerZ = z_offset * -space_scale
        
        let selectedPlanetId = bodies.[current_planet].id
        
        let projection = currentProjection.ToString ()

        let yearsPassed = timePassed / (60.0 * 60.0 * 24.0 * 365.0)
        
        let text =
            sprintf "player position {%f, %f, %f} scale %f time_scale %f planet %s projection %s passed %.2f years"
                playerX playerY playerZ
                space_scale dt_scale
                selectedPlanetId
                projection
                yearsPassed

        if showInfo then
            secondBuffer.DrawString (text, font, brush, rect)
            List.iteri
                (fun index text ->
                    let rect = new RectangleF (0.0f, float32 (index + 1) * 20.0f, 0.0f, 0.0f)
                    secondBuffer.DrawString (text, font, brush, rect))
                (printParameters bodies)

        secondBuffer.DrawImageUnscaled(orbitsBitmap, 0, 0)
        firstBuffer.DrawImageUnscaled(secondBitmap, 0, 0)

        ()
    
    let configFileName =
        if argv.Length >= 1 then argv.[0]
        else "solar_system.xml"
    bodies <- parseFile configFileName

    form.KeyPress.Add (keypressEvent bodies)

    use timer = new Timer ()
    timer.Interval <- int $ interval * 1000.0

    secondBuffer.Clear Color.Black

    use font = new Font ("Consolas", 8.0f)
    use brush = new SolidBrush (Color.White)

    timer.Tick.Add (tick font brush)
    timer.Start ()

    Application.Run form

    timer.Stop ()        
    0
