module Solar.Main

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

let doc = "
Options:
  --help        this message
  -c, --config  path to configuration file
  -s, --step    simultation time step (in seconds)
  -w, --width   set width
  -h, --height  set height

Controls:
  | Keys    |                         |
  |---------|-------------------------|
  | W A S D | move                    |
  | Q E     | axis move               |
  | + -     | control time speed      |
  | , .     | control scale           |
  | 8       | move to selected planet |
  | 9 0     | select planet           |
  | P       | change projection       |
  | O       | toggle information      |
  | I       | toggle orbits           |
  | Space   | toggle pause            |
"

let rec parseArgv (argv : string list) : unit =
    match argv with
    | "--config" :: name :: tail
    | "-c" :: name :: tail ->
        bodies <- parseFile name
        parseArgv tail

    | "--step" :: stepString :: tail
    | "-s" :: stepString :: tail ->
        let (correct, stepValue) = Double.TryParse stepString
        if not correct then
            printfn "incorrect ‘--step’"
            exit 0

        step <- stepValue
        parseArgv tail

    | "--help" :: _ ->
        Console.WriteLine (doc)
        exit 0

    | "--width" :: widthString :: tail
    | "-w" :: widthString :: tail ->
        let (correct, newWidth) = Int32.TryParse widthString
        if not correct then
            printfn "incorrect ‘--width’"
            exit 0
        
        width <- newWidth
        parseArgv tail

    | "--height" :: heightString :: tail
    | "-h" :: heightString :: tail ->
        let (correct, newHeight) = Int32.TryParse heightString
        if not correct then
            printfn "incorrect ‘--height’"
            exit 0
        
        height <- newHeight
        parseArgv tail

    | [] -> ()

    | head :: _ ->
        printfn "unknown argument ‘%s’" head
        exit 0

[<EntryPoint>]
let main argv =    
    let mutable currentPlanet = 0

    let mutable paused = false
    let mutable showInfo = false
    let mutable showOrbits = false
    let mutable timePassed = 0.0

    parseArgv $ List.ofArray argv

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
                sprintf "pos %s, speed %s, acc %s, id %s"
                    (string here.pos)
                    (string here.speed)
                    (string here.acc)
                    here.id)
            bodies
    
    let moveToPlanet n =
        xOffset <-
            (bodies.[n].pos.x / -spaceScale)
        yOffset <-
            (bodies.[n].pos.y / -spaceScale)
    
    let keypressEvent (bodies : Body list) (e : KeyPressEventArgs) =
        let c = Char.ToLower e.KeyChar
        if isMoving c then orbitsBuffer.Clear Color.Transparent
        match c with
        | DOWN -> yOffset <- yOffset - moveOffset
        | UP -> yOffset <- yOffset + moveOffset
        | RIGHT -> xOffset <- xOffset - moveOffset
        | LEFT -> xOffset <- xOffset + moveOffset
        | Z_DOWN -> zOffset <- zOffset - moveOffset
        | Z_UP -> zOffset <- zOffset + moveOffset

        | SCALE_UP -> spaceScale <- spaceScale * spaceScaleOffset
        | SCALE_DOWN -> spaceScale <- spaceScale / spaceScaleOffset

        | PLANET_NEXT ->
            if currentPlanet + 1 = bodies.Length then ()
            else currentPlanet <- currentPlanet + 1
        | PLANET_BACK ->
            if currentPlanet = 0 then ()
            else currentPlanet <- currentPlanet - 1
        | PLANET_GO ->
            moveToPlanet currentPlanet

        | CHANGE_PROJECTION ->
            currentProjection <-
                match currentProjection with
                | XY -> XZ
                | XZ -> YZ
                | YZ -> XY

        | DT_SCALE_UP -> dtScale <- dtScale * dtScaleOffset
        | DT_SCALE_DOWN -> dtScale <- dtScale / dtScaleOffset
        | PAUSE -> paused <- not paused

        | TOGGLE_INFO -> showInfo <- not showInfo
        | TOGGLE_ORBITS -> showOrbits <- not showOrbits
        | _ -> ()

    let tick (font : Font) (brush : Brush) _ =
        let dt = 24.0 * 60.0 * 60.0 * interval * dtScale

        if not paused then
            let dtAmount = int $ dt / step

            for _ in 0 .. dtAmount do
                bodies <- List.map (updateBody bodies step) bodies
            bodies <- List.map (updateBody bodies $ dt % step) bodies

            timePassed <- timePassed + dt

        secondBuffer.Clear Color.Black
        if not showOrbits then orbitsBuffer.Clear Color.Transparent

        List.iter (drawBodyPoint orbitsBuffer) bodies
        secondBuffer.DrawImageUnscaled(orbitsBitmap, 0, 0)

        List.iter
            (fun b ->
                drawBody secondBuffer b
                drawBodyTag secondBuffer b)
            bodies

        let rect = RectangleF (0.0f, 0.0f, 0.0f, 0.0f)
        let playerX = xOffset * -spaceScale
        let playerY = yOffset * -spaceScale
        let playerZ = zOffset * -spaceScale

        let selectedPlanetId = bodies.[currentPlanet].id

        let projection = currentProjection.ToString ()

        let yearsPassed = timePassed / (60.0 * 60.0 * 24.0 * 365.0)

        let text =
            sprintf "player position {%f, %f, %f}, scale %f, time_scale %f, planet %s, projection %s, passed %.2f years"
                playerX playerY playerZ
                spaceScale dtScale
                selectedPlanetId
                projection
                yearsPassed

        if showInfo then
            secondBuffer.DrawString (text, font, brush, rect)
            List.iteri
                (fun index text ->
                    let rect = RectangleF (0.0f, float32 (index + 1) * 20.0f, 0.0f, 0.0f)
                    secondBuffer.DrawString (text, font, brush, rect))
                (printParameters bodies)

        firstBuffer.DrawImageUnscaled(secondBitmap, 0, 0)
        ()

    if List.isEmpty bodies then bodies <- parseFile "solar_system.xml"

    form.KeyPress.Add (keypressEvent bodies)

    use timer = new Timer ()
    timer.Interval <- int $ interval * 1000.0

    use font = new Font ("Consolas", 8.0f)
    use brush = new SolidBrush (Color.White)

    timer.Tick.Add (tick font brush)
    timer.Start ()

    Application.Run form

    timer.Stop ()
    0
