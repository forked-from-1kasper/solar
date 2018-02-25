namespace Solar

open System.Drawing

module Graphics =
    open Solar.Physics
    
    let drawBody (g : Graphics) (b : Body) =
        use brush = new SolidBrush (b.visibleColor)
        let radius = float32 b.visibleRadius

        let scaledPos = scale b.pos

        g.FillEllipse (brush, new RectangleF (float32 scaledPos.x - radius,
                                              float32 scaledPos.y - radius,
                                              2.0f * radius,
                                              2.0f * radius))
