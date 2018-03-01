namespace Solar

module Physics =
    open Solar.Constants
    let gravitionalForce (first : Body) (second : Body) =
        let distance = (second.pos - first.pos).Abs
        (second.sgp / (distance ** 3.0)) * (second.pos - first.pos)

    let space_scale_offset = 2.0
    let mutable space_scale = 10.0 ** 9.0

    let dt_scale_offset = 2.0
    let mutable dt_scale = 1.0

    let move_offset = 10.0
    let mutable x_offset = 0.0
    let mutable y_offset = 0.0
    let mutable z_offset = 0.0

    let mutable step = 60.0 * 60.0
    
    let scale (v : Vector) =
        { x = (v.x / space_scale) + x_offset;
          y = (v.y / space_scale) + y_offset;
          z = (v.z / space_scale) + z_offset }

    let updateBody (bodies : Body list) (dt : float) (here : Body) =
        let allWithoutMe = List.filter ((<>) here) bodies
        let acc =
            List.sumBy (gravitionalForce here) allWithoutMe

        let speed = here.speed + (acc * dt)
        let pos = here.pos + (speed * dt)

        { acc = acc;
          speed = speed;
          pos = pos;
          sgp = here.sgp;
          id = here.id;

          visibleRadius = here.visibleRadius;
          visibleColor = here.visibleColor }
