namespace Map
{
    public class MapStage : Stage
    {
        private Map map;

        protected override void OnEnable()
        {
            base.OnEnable();
            map?.StartNavigation();
        }

        protected override bool HasAllComponents()
        {
            if (base.HasAllComponents() && map) return true;
            map = GetComponentInChildren<Map>(true);
            return base.HasAllComponents() && map;
        }
    }
}
