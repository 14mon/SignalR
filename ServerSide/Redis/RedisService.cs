using StackExchange.Redis;

namespace Chat.Redis
{
    public class RedisService
    {
        private readonly ConnectionMultiplexer _readConnection;
        private readonly ConnectionMultiplexer _writeConnection;

        public RedisService(string readConnection, string writeConnection, string instanceName)
        {
            var readOptions = ConfigurationOptions.Parse(readConnection);
            readOptions.AbortOnConnectFail = false;  // Allow retrying

            readOptions.ClientName = instanceName;
            _readConnection = ConnectionMultiplexer.Connect(readOptions);

            var writeOptions = ConfigurationOptions.Parse(writeConnection);
            writeOptions.AbortOnConnectFail = false; // Allow retrying

            writeOptions.ClientName = instanceName;
            _writeConnection = ConnectionMultiplexer.Connect(writeOptions);
        }

        public IDatabase GetRedisReadDatabase()
        {
            return _readConnection.GetDatabase();
        }

        public IDatabase GetRedisWriteDatabase()
        {
            return _writeConnection.GetDatabase();
        }

        public void Close()
        {
            _readConnection.Close();
            _readConnection.Dispose();

            _writeConnection.Close();
            _writeConnection.Dispose();
        }
    }
}