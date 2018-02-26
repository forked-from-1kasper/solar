namespace Solar

module Physics =
    open Solar.Constants
    let gravitionalForce (first : Body) (second : Body) =
        let distance = (second.pos - first.pos.Neg).Abs
        G * (second.m / (distance ** 3.0)) * (second.pos - first.pos)

    let scale (v : Vector) =
        let k = 10.0 ** 9.0
        { x = (v.x / k) + (float width / 2.0);
          y = (v.y / k) + (float height / 2.0);
          z = (v.z / k)}

    let updateBody (bodies : Body list) (dt : float) (here : Body) =
        let allWithoutMe = List.filter ((<>) here) bodies
        let acc =
            List.sumBy (gravitionalForce here) allWithoutMe
        let speed = here.speed + (acc * dt)
        let pos = here.pos + (speed * dt)

        { acc = acc;
          speed = speed;
          pos = pos;
          m = here.m;
          id = here.id;

          visibleRadius = here.visibleRadius;
          visibleColor = here.visibleColor }
