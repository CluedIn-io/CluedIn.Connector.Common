namespace CluedIn.Connector.Common.Features
{
    public interface IFeatureStore
    {
        void SetFeature<T>(T instance);

        T GetFeature<T>();
    }
}
