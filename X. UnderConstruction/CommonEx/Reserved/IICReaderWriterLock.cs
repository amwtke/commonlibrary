///*
// * A Wrapper of ReaderWriterLock
// * 
// * Author:	Lei Gao
// * Created:	2008-03-06
// * Modified:
// *			2009-09-16 改用3.5版本的ReaderWriterLockSlim实现
// *			
// */
//using System;
//using System.Threading;
//using System.Collections.Generic;
//using System.Text;

//namespace Imps.Services.CommonV4
//{
//    public class IICReaderWriterLock: IDisposable
//    {
//        private ReaderWriterLockSlim _innerLock;
//        private int _timeout;

//        public IICReaderWriterLock(): this(-1)
//        {
//        }

//        public IICReaderWriterLock(int millisecondTimeout)
//        {
//            _innerLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
//            _timeout = millisecondTimeout;
//        }

//        public IICLockRegion LockForRead(int millisecondTimeout)
//        {
//            IICLockRegion region = new IICLockRegion(_innerLock, IICLockMode.ReaderLock, millisecondTimeout);
//            return region;
//        }

//        public IICLockRegion LockForUpgradeableRead(int millisecondTimeout)
//        {
//            IICLockRegion region = new IICLockRegion(_innerLock, IICLockMode.UpgradeableReadLock, millisecondTimeout);
//            return region;
//        }

//        public IICLockRegion LockForWrite(int millisecondTimeout)
//        {
//            IICLockRegion region = new IICLockRegion(_innerLock, IICLockMode.WriterLock, millisecondTimeout);
//            return region;
//        }

//        public IICLockRegion LockForRead()
//        {
//            return LockForRead(_timeout);
//        }

//        public IICLockRegion LockForUpgradeableRead()
//        {
//            return LockForUpgradeableRead(_timeout);
//        }

//        public IICLockRegion LockForWrite()
//        {
//            return LockForWrite(_timeout);
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