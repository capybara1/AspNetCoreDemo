namespace AspNetCoreDemo.Middleware.Authorization.Contracts
{
    public class UserId
    {
        private readonly string _value;

        public UserId(string value)
        {
            _value = value ?? throw new System.ArgumentNullException(nameof(value));
        }

        public static explicit operator string(UserId userId)
        {
            return userId._value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
