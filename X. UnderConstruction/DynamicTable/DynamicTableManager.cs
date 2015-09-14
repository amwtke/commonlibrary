//using System;
//using System.Threading;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Imps.Services.HA;
//using Imps.Services.CommonV4.IICConfig;

//namespace Imps.Services.CommonV4
//{
//    public static class DynamicTableManager
//    {
//        public static void Initialize()
//        {
//            Thread trd = new Thread(RefreshProc);
//            trd.IsBackground = true;
//            trd.Name = "DynamicTableManager.RefreshProc";
//            trd.Start();
//        }

//        public static IICCodeTable<K, V> GetCodeTable<K, V>(string tableName, Action<IICCodeTable<K, V>> onUpdate) where V: IICCodeTableItem
//        {
//            ConfigCodeTableUpdater<K, V> updater = new ConfigCodeTableUpdater<K, V>(tableName, onUpdate);
//            _updaters.Add(updater);
			
//            IICCodeTable<K, V> table = IICConfigurationManager.Imp.GetCodeTable<K, V>(tableName, onUpdate);
//            updater.Version = table.Version;
//            return table;
//        }

//        //public static void UpdateTable_(K key, V value)
//        //{
//        //}

//        private static void RefreshProc()
//        {
//            while (true) {
//                try {
//                    RpcList<string> args = new RpcList<string>();
//                    foreach (ConfigUpdater updater in _updaters) {
//                        args.Value.Add(updater.Path);
//                    }

//                    RpcDictionary<string, DateTime> versions = RpcProxyFactory.Call<IHACenterService, RpcList<string>, RpcDictionary<string, DateTime>>(
//                        HAEnviorment.CenterUrl,
//                        "RefreshTableVersion",
//                        args
//                        );

//                    foreach (ConfigUpdater updater in _updaters) {
//                        DateTime newVersion;
//                        if (!versions.Value.TryGetValue(updater.Path, out newVersion)) {
//                            throw new Exception();
//                        }
//                        if (newVersion > updater.Version) {
//                            try {
//                                updater.OnUpdate();
//                            } catch (Exception ex) {
//                                SystemLog.Error(LogEventID.ConfigFailed, ex, "DynamicTable Update Failed");
//                            }
//                        }
//                    }
//                } catch (Exception ex) {
//                }
//            }
//        }

//        private static SafeList<ConfigUpdater> _updaters = null;
//        private static ITracing _tracing = TracingManager.GetTracing(typeof(DynamicTableManager));
//    }
//}
