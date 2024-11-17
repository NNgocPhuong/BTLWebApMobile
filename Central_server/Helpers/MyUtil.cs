namespace E_Commerce.Helpers
{
    public class MyUtil
    {
        public static string GenarateRandomKey(int length = 5)
        {
            string result = "";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                result += (char)random.Next(65, 90);
            }
            return result;
        }
    }
}
