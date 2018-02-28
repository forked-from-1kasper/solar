namespace Solar

open System.Drawing

type Body =
    { acc : Vector;
      speed : Vector;
      pos : Vector;
      sgp : float;
      id : string;

      visibleRadius : float;
      visibleColor : Color }
