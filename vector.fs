namespace Solar

type Vector =
    { x : float;
      y : float;
      z : float }
    with
        member v.Neg =
            { x = -v.x; y = -v.y; z = -v.z }

        member v.Abs =
            (v.x ** 2.0 + v.y ** 2.0 + v.z ** 2.0) ** (1.0/2.0)

        static member (+) (a : Vector, b) =
            { x = a.x + b.x;
              y = a.y + b.y;
              z = a.z + b.z }

        static member (-) (a : Vector, b : Vector) =
            a + b.Neg

        static member (*) (k : float, a : Vector) =
            { x = k * a.x;
              y = k * a.y;
              z = k * a.z }

        static member (*) (a : Vector, k : float) =
            k * a

        static member (/) (a : Vector, k : float) =
            a * (1.0/k)

        static member Zero = { x = 0.0; y = 0.0; z = 0.0 }

        override v.ToString () =
            sprintf "{%.2f, %.2f, %.2f}" v.x v.y v.z
