namespace Quark.Cloudflare.Adapter.Interfaces
{
    public interface ICloudflareAdapter
    {
        public bool Purge(string url);
    }
}
