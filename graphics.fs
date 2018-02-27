namespace Solar

open System.Drawing

module Graphics =
    open Solar.Improvements
    open Solar.Physics
    
    let drawBody (g : Graphics) (b : Body) =
        use brush = new SolidBrush (b.visibleColor)
        let radius =
            let realScaled = float32 $ b.visibleRadius / space_scale
            if realScaled <= 0.5f then 0.5f
            else realScaled

        let scaledPos = scale b.pos

        let x = float32 scaledPos.x - radius
        let y = float32 scaledPos.y - radius
        g.FillEllipse (brush, new RectangleF (x, y,
                                              2.0f * radius,
                                              2.0f * radius))

        use font = new Font ("Consolas", 8.0f)
        use fontBrush = new SolidBrush (Color.White)
        let rect = new RectangleF (x, y, 0.0f, 0.0f)
        g.DrawString (b.id, font, fontBrush, rect)
