namespace Solar

open System.Drawing

module Graphics =
    open Solar.Improvements
    open Solar.Physics
    open Solar.Constants

    type Projection = XY | XZ | YZ
    let mutable currentProjection = XY

    let getCoors (b : Body) =
        let radius =
            let realScaled = float32 $ b.visibleRadius / space_scale
            if realScaled <= 0.5f then 0.5f
            else realScaled
        
        let scaledPos = scale b.pos
        let (a, b) =
            match currentProjection with
            | XY -> (scaledPos.x, scaledPos.y)
            | XZ -> (scaledPos.x, scaledPos.z)
            | YZ -> (scaledPos.y, scaledPos.z)
        (float32 a + (float32 width / 2.0f),
         float32 b + (float32 height / 2.0f),
         radius)
    
    let drawBody (g : Graphics) (b : Body) =
        use brush = new SolidBrush (b.visibleColor)

        let (firstCoor, secondCoor, radius) = getCoors b
            
        g.FillEllipse (brush, new RectangleF (firstCoor - radius,
                                              secondCoor - radius,
                                              2.0f * radius,
                                              2.0f * radius))

    let drawBodyPoint (g : Graphics) (b : Body) =
        use brush = new SolidBrush (b.visibleColor)

        let (firstCoor, secondCoor, radius) = getCoors b
            
        g.FillRectangle (brush, firstCoor, secondCoor, 1.0f, 1.0f)

    let drawBodyTag (g : Graphics) (b : Body) =
        let (firstCoor, secondCoor, _) = getCoors b
        
        use font = new Font ("Consolas", 8.0f)
        use fontBrush = new SolidBrush (Color.White)
        let rect = new RectangleF (firstCoor, secondCoor, 0.0f, 0.0f)
        g.DrawString (b.id, font, fontBrush, rect)
