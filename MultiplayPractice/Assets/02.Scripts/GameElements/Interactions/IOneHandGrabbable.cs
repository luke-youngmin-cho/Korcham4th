namespace MP.GameElements.Interactions
{
    public interface IOneHandGrabbable
    {
        void Grab(IOneHandGrabber grabber);

        void Ungrab();
    }
}
