namespace Rt.Framework.Api
{
	public interface IUrlRepository
	{
		string[] GetUrls();
		void AddUrl(string newUrl);
		void RenameUrl(string oldUrl, string newUrl);
		void DeleteUrl(string url);
	}
}
