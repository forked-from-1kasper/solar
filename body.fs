namespace Solar

open System.Drawing

type Body =
    { acc : Vector;
      speed : Vector;
      pos : Vector;
      m : float;
      id : string;

      visibleRadius : float;
      visibleColor : Color }
