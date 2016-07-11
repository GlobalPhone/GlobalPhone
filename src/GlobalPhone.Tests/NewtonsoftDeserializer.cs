using Makrill;
namespace GlobalPhone.Tests
{
    public class NewtonsoftDeserializer:IDeserializer
	{
		private static readonly JsonConvert jsonConvert = new JsonConvert();
		public NewtonsoftDeserializer ()
		{
		}

		public object[] Deserialize (string text)
		{
            return jsonConvert.Deserialize<object[]> (text);
		}
	}
}

