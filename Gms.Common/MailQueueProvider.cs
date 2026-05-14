using Gms.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Common
{
    public class MailQueueProvider
    {
        public readonly static ConcurrentQueue<MailBox> MailQueue = new ConcurrentQueue<MailBox>();

        public static void EnqueueMailBox(MailBox mailBox)
        {
            MailQueue.Enqueue(mailBox);
        }
        public static bool DequeueMailBox(out MailBox mailBox)
        {
            return MailQueue.TryDequeue(out mailBox);
        }
    }
}
