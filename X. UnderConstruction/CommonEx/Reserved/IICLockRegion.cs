///*
// * A Wrapper of ReaderWriterLock
// * 
// * Author:	Lei Gao 
// * Created:	2008-03-06
// * Modified:
// *			2009-09-16 改用3.5版本的ReaderWriterLockSlim实现
// */
//using System;
//using System.Threading;
//using System.Collections.Generic;
//using System.Text;

//namespace Imps.Services.CommonV4
//{
//    public enum IICLockMode
//    {
//        ReaderLock,
//        UpgradeableReadLock, 
//        UpdatedWriterLock,
//        WriterLock,
//    }

//    public class IICLockRegion: IDisposable
//    {
//        private ReaderWriterLockSlim _innerLock;
//        private IICLockMode _lockMode;
//        private int _msTimeout;

//        public IICLockRegion(ReaderWriterLockSlim innerLock, IICLockMode mode, int millisecondTimeout)
//        {
//            _innerLock = innerLock;
//            _lockMode = mode;
//            _msTimeout = millisecondTimeout;

//            if (_msTimeout < 0) {
//                switch (mode) {
//                    case IICLockMode.ReaderLock:
//                        _innerLock.EnterReadLock();
//                        break;
//                    case IICLockMode.UpgradeableReadLock:
//                        _innerLock.EnterUpgradeableReadLock();
//                        break;
//                    case IICLockMode.WriterLock:
//                        _innerLock.EnterWriteLock();
//                        break;
//                    default:
//                        throw new NotSupportedException("Unexcepted LockMode: " + mode);
//                }
//            } else {
//                bool successed;
//                switch (mode) {
//                    case IICLockMode.ReaderLock:
//                        successed = _innerLock.TryEnterReadLock(_msTimeout);
//                        break;
//                    case IICLockMode.UpgradeableReadLock:
//                        successed = _innerLock.TryEnterUpgradeableReadLock(_msTimeout);
//                        break;
//                    case IICLockMode.WriterLock:
//                        successed = _innerLock.TryEnterWriteLock(_msTimeout);
//                        break;
//                    default:
//                        throw new NotSupportedException("Unexcepted LockMode: " + mode);
//                }
//                if (!successed) {
//                    string msg = string.Format("ReadWriterLock<{0}> Timeout with {1}ms :", mode, _msTimeout);
//                    throw new TimeoutException(msg); 
//                }
//            }
//        }

//        public void Upgrade()
//        {
//            if (_lockMode != IICLockMode.UpgradeableReadLock) {
//                throw new InvalidOperationException("lock mode can't upgraded:" + _lockMode);
//            }

//            if (_msTimeout > 0) {
//                bool successed = _innerLock.TryEnterWriteLock(_msTimeout);

//                if (!successed) {
//                    string msg = string.Format("ReadWriterLock<UpdateWriterLock> Timeout with {0}ms :", _msTimeout);
//                    throw new TimeoutException(msg);
//                }
//            } else {
//                _innerLock.EnterWriteLock();
//            }
//            _lockMode = IICLockMode.UpdatedWriterLock;
//        }

//        public void Dispose()
//        {
//            switch (_lockMode) {
//                case IICLockMode.ReaderLock:
//                    _innerLock.ExitReadLock();
//                    break;
//                case IICLockMode.UpgradeableReadLock:
//                    _innerLock.ExitUpgradeableReadLock();
//                    break;
//                case IICLockMode.UpdatedWriterLock:
//                    _innerLock.ExitWriteLock();
//                    _innerLock.ExitUpgradeableReadLock();
//                    break;
//                case IICLockMode.WriterLock:
//                    _innerLock.ExitWriteLock();
//                    break;
//            }
//        }
//    }
//}


///*
//'JAPH V1.0';$/=0xE0;$^F=1<<5;$;=$^F>>3;$,=
//'Nywx$           /\/\       x7Fy~'.
//'Ersxl      ,"~~~    \      kde~b'.
//'iv$Ti     /        @ \_    ox*Zo'.
//'vp$Le  ~~|           __0   f*Bki'.
//'goiv2     \||||--||-|/     ax0ox';
//{$_=chr(ord(substr($,,$"++,1))-$;);print;
//$"=($"&$/)+$^Fif$"%$^F>$;;redo if$"<=$^F*$;+$;;}
//*/