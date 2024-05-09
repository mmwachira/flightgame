public class ObstacleFull : Obstacle
{
    public override void Spawn(TrackSegment segment, float t)
    {
        segment.GetPointAt(t, out var position, out var rotation);

        Instantiate(gameObject, position, rotation, segment.ContainerObstacles);
    }
}