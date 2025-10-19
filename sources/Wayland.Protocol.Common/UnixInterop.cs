using System.Runtime.InteropServices;

namespace Wayland.Protocol.Common;

public class UnixInterop
{
    public const int SOL_SOCKET = 1;
    public const int SCM_RIGHTS = 0x01;

    [StructLayout(LayoutKind.Sequential)]
    public struct iovec
    {
        public IntPtr iov_base;
        public IntPtr iov_len;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct msghdr
    {
        public IntPtr msg_name;
        public uint msg_namelen;
        public IntPtr msg_iov;
        public IntPtr msg_iovlen;
        public IntPtr msg_control;
        public IntPtr msg_controllen;
        public int msg_flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct cmsghdr
    {
        public IntPtr cmsg_len;
        public int cmsg_level;
        public int cmsg_type;
    }

    [DllImport("libc", SetLastError = true)]
    public static extern int sendmsg(int sockfd, ref msghdr msg, int flags);

    [DllImport("libc", SetLastError = true)]
    public static extern int recvmsg(int sockfd, ref msghdr msg, int flags);
    
    
    public static int CMsgAlign(int length) =>
        (length + IntPtr.Size - 1) & ~(IntPtr.Size - 1);

    public static int CMsgLength(int dataLength) =>
        CMsgAlign(Marshal.SizeOf<UnixInterop.cmsghdr>()) + dataLength;

    public static int CMsgSpace(int dataLength) =>
        CMsgAlign(Marshal.SizeOf<UnixInterop.cmsghdr>()) + CMsgAlign(dataLength);
}