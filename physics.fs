namespace Solar

module Physics =
    let gravitionalForce (first : Body) (second : Body) =
        let distance = (second.pos - first.pos).Abs
        (second.μ / (distance ** 3.0)) * (second.pos - first.pos)

    let spaceScaleOffset = 2.0
    let mutable spaceScale = 10.0 ** 9.0

    let dtScaleOffset = 2.0
    let mutable dtScale = 1.0

    let moveOffset = 10.0
    let mutable xOffset = 0.0
    let mutable yOffset = 0.0
    let mutable zOffset = 0.0

    let mutable step = 60.0 * 60.0
    
    let scale (v : Vector) =
        { x = (v.x / spaceScale) + xOffset;
          y = (v.y / spaceScale) + yOffset;
          z = (v.z / spaceScale) + zOffset }

    let updateBody (bodies : Body list) (dt : float) (here : Body) =
        let allWithoutMe = List.filter ((<>) here) bodies
        let acc =
            List.sumBy (gravitionalForce here) allWithoutMe

        let speed = here.speed + (acc * dt)
        let pos = here.pos + (speed * dt)

        { acc = acc;
          speed = speed;
          pos = pos;
          μ = here.μ;
          id = here.id;

          visibleRadius = here.visibleRadius;
          visibleColor = here.visibleColor }
