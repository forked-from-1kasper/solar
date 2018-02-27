namespace Solar

open System.Drawing

module Graphics =
    open Solar.Improvements
    open Solar.Physics
    open Solar.Constants

    type Projection = XY | XZ | YZ
    let mutable currentProjection = XY
    
    let drawBody (g : Graphics) (b : Body) =
        use brush = new SolidBrush (b.visibleColor)
        let radius =
            let realScaled = float32 $ b.visibleRadius / space_scale
            if realScaled <= 0.5f then 0.5f
            else realScaled

        let scaledPos = scale b.pos

        let firstCoor =
            let value =
                match currentProjection with
                | XY -> scaledPos.x
                | XZ -> scaledPos.x
                | YZ -> scaledPos.y
            float32 value - radius + (float32 width / 2.0f)

        let secondCoor =
            let value =
                match currentProjection with
                | XY -> scaledPos.y
                | XZ -> scaledPos.z
                | YZ -> scaledPos.z
            float32 value - radius + (float32 height / 2.0f)
            
        g.FillEllipse (brush, new RectangleF (firstCoor, secondCoor,
                                              2.0f * radius,
                                              2.0f * radius))

        use font = new Font ("Consolas", 8.0f)
        use fontBrush = new SolidBrush (Color.White)
        let rect = new RectangleF (firstCoor, secondCoor, 0.0f, 0.0f)
        g.DrawString (b.id, font, fontBrush, rect)
