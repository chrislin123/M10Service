using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//using FluentFTP;

namespace M10.lib
{
    class FluentFTPHelper
    {

        ///// <summary>
        ///// FTP操作类(FluentFTP封装)
        ///// </summary>
        //public class FtpHelper
        //{
        //    #region 相关参数
        //    /// <summary>
        //    /// FtpClient
        //    /// </summary>
        //    private FtpClient ftpClient = null;
        //    /// <summary>
        //    /// FTP IP地址(127.0.0.1)
        //    /// </summary>
        //    private string strFtpUri = string.Empty;
        //    /// <summary>
        //    /// FTP端口
        //    /// </summary>
        //    private int intFtpPort = 21;
        //    /// <summary>
        //    /// FTP用户名
        //    /// </summary>
        //    private string strFtpUserID = string.Empty;
        //    /// <summary>
        //    /// FTP密码
        //    /// </summary>
        //    private string strFtpPassword = string.Empty;
        //    /// <summary>
        //    /// 重试次数
        //    /// </summary>
        //    private int intRetryTimes = 3;
        //    /// <summary>
        //    /// FTP工作目录
        //    /// </summary>
        //    private string _workingDirectory = string.Empty;
        //    /// <summary>
        //    /// FTP工作目录
        //    /// </summary>
        //    public string WorkingDirectory
        //    {
        //        get
        //        {
        //            return _workingDirectory;
        //        }
        //    }
        //    #endregion

        //    #region 构造函数
        //    /// <summary>
        //    /// 构造函数
        //    /// </summary>
        //    /// <param name="ftpConfig">FTP配置封装</param>
        //    public FtpHelper(FtpConfig ftpConfig)
        //    {
        //        this.strFtpUri = ftpConfig.str_FtpUri;
        //        this.intFtpPort = ftpConfig.int_FtpPort;
        //        this.strFtpUserID = ftpConfig.str_FtpUserID;
        //        this.strFtpPassword = ftpConfig.str_FtpPassword;
        //        this.intRetryTimes = ftpConfig.int_RetryTimes;
        //        //创建ftp客户端
        //        GetFtpClient();
        //    }

        //    /// <summary>
        //    /// 构造函数
        //    /// </summary>
        //    /// <param name="host">FTP IP地址</param>
        //    /// <param name="port">FTP端口</param>
        //    /// <param name="username">FTP用户名</param>
        //    /// <param name="password">FTP密码</param>
        //    public FtpHelper(string host, int port, string username, string password)
        //    {
        //        strFtpUri = host;
        //        intFtpPort = port;
        //        strFtpUserID = username;
        //        strFtpPassword = password;
        //        //创建ftp客户端
        //        GetFtpClient();
        //    }
        //    #endregion

        //    #region 创建ftp客户端
        //    /// <summary>
        //    /// 创建ftp客户端
        //    /// </summary>
        //    private void GetFtpClient()
        //    {
        //        if (CheckPara())
        //        {
        //            try
        //            {
        //                ftpClient = new FtpClient(strFtpUri, intFtpPort, strFtpUserID, strFtpPassword);
        //                ftpClient.RetryAttempts = intRetryTimes;
        //            }
        //            catch (Exception ex)
        //            {
        //                Log4NetUtil.Error(this, "GetFtpClient->创建ftp客户端异常:" + ex.ToString());
        //            }
        //        }
        //    }
        //    #endregion

        //    #region 校验参数
        //    /// <summary>
        //    /// 校验参数
        //    /// </summary>
        //    /// <returns></returns>
        //    private bool CheckPara()
        //    {
        //        bool boolResult = true;

        //        if (string.IsNullOrEmpty(strFtpUri))
        //        {
        //            Log4NetUtil.Error(this, "CheckPara->FtpUri为空");
        //            return false;
        //        }
        //        if (string.IsNullOrEmpty(strFtpUserID))
        //        {
        //            Log4NetUtil.Error(this, "CheckPara->FtpUserID为空");
        //            return false;
        //        }
        //        if (string.IsNullOrEmpty(strFtpPassword))
        //        {
        //            Log4NetUtil.Error(this, "CheckPara->FtpPassword为空");
        //            return false;
        //        }
        //        if (intFtpPort == 0 || intFtpPort == int.MaxValue || intFtpPort == int.MinValue)
        //        {
        //            Log4NetUtil.Error(this, "CheckPara->intFtpPort异常:" + intFtpPort.ToString());
        //            return false;
        //        }
        //        return boolResult;
        //    }
        //    #endregion

        //    #region FTP是否已连接
        //    /// <summary>
        //    /// FTP是否已连接
        //    /// </summary>
        //    /// <returns></returns>
        //    public bool isConnected()
        //    {
        //        bool result = false;
        //        if (ftpClient != null)
        //        {
        //            result = ftpClient.IsConnected;
        //        }
        //        return result;
        //    }
        //    #endregion

        //    #region 连接FTP
        //    /// <summary>
        //    /// 连接FTP
        //    /// </summary>
        //    /// <returns></returns>
        //    public bool Connect()
        //    {
        //        bool result = false;
        //        if (ftpClient != null)
        //        {
        //            if (ftpClient.IsConnected)
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                ftpClient.Connect();
        //                return true;
        //            }
        //        }
        //        return result;
        //    }
        //    #endregion

        //    #region 断开FTP
        //    /// <summary>
        //    /// 断开FTP
        //    /// </summary>
        //    public void DisConnect()
        //    {
        //        if (ftpClient != null)
        //        {
        //            if (ftpClient.IsConnected)
        //            {
        //                ftpClient.Disconnect();
        //            }
        //        }
        //    }
        //    #endregion

        //    #region 取得文件或目录列表
        //    /// <summary>
        //    /// 取得文件或目录列表
        //    /// </summary>
        //    /// <param name="remoteDic">远程目录</param>
        //    /// <param name="type">类型:file-文件,dic-目录</param>
        //    /// <returns></returns>
        //    public List<string> ListDirectory(string remoteDic, string type = "file")
        //    {
        //        List<string> list = new List<string>();
        //        type = type.ToLower();

        //        try
        //        {
        //            if (Connect())
        //            {
        //                FtpListItem[] files = ftpClient.GetListing(remoteDic);
        //                foreach (FtpListItem file in files)
        //                {
        //                    if (type == "file")
        //                    {
        //                        if (file.Type == FtpFileSystemObjectType.File)
        //                        {
        //                            list.Add(file.Name);
        //                        }
        //                    }
        //                    else if (type == "dic")
        //                    {
        //                        if (file.Type == FtpFileSystemObjectType.Directory)
        //                        {
        //                            list.Add(file.Name);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        list.Add(file.Name);
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log4NetUtil.Error(this, "ListDirectory->取得文件或目录列表 异常:" + ex.ToString());
        //        }
        //        finally
        //        {
        //            DisConnect();
        //        }

        //        return list;
        //    }
        //    #endregion

        //    #region 上传单文件
        //    /// <summary>
        //    /// 上传单文件
        //    /// </summary>
        //    /// <param name="localPath">本地路径(@"D:\abc.txt")</param>
        //    /// <param name="remoteDic">远端目录("/test")</param>
        //    /// <returns></returns>
        //    public bool UploadFile(string localPath, string remoteDic)
        //    {
        //        bool boolResult = false;
        //        FileInfo fileInfo = null;

        //        try
        //        {
        //            //本地路径校验
        //            if (!File.Exists(localPath))
        //            {
        //                Log4NetUtil.Error(this, "UploadFile->本地文件不存在:" + localPath);
        //                return boolResult;
        //            }
        //            else
        //            {
        //                fileInfo = new FileInfo(localPath);
        //            }
        //            //远端路径校验
        //            if (string.IsNullOrEmpty(remoteDic))
        //            {
        //                remoteDic = "/";
        //            }
        //            if (!remoteDic.StartsWith("/"))
        //            {
        //                remoteDic = "/" + remoteDic;
        //            }
        //            if (!remoteDic.EndsWith("/"))
        //            {
        //                remoteDic += "/";
        //            }

        //            //拼接远端路径
        //            remoteDic += fileInfo.Name;

        //            if (Connect())
        //            {
        //                using (FileStream fs = fileInfo.OpenRead())
        //                {
        //                    //重名覆盖
        //                    boolResult = ftpClient.Upload(fs, remoteDic, FtpExists.Overwrite, true);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log4NetUtil.Error(this, "UploadFile->上传文件 异常:" + ex.ToString() + "|*|localPath:" + localPath);
        //        }
        //        finally
        //        {
        //            DisConnect();
        //        }

        //        return boolResult;
        //    }
        //    #endregion

        //    #region 上传多文件
        //    /// <summary>
        //    /// 上传多文件
        //    /// </summary>
        //    /// <param name="localFiles">本地路径列表</param>
        //    /// <param name="remoteDic">远端目录("/test")</param>
        //    /// <returns></returns>
        //    public int UploadFiles(IEnumerable<string> localFiles, string remoteDic)
        //    {
        //        int count = 0;
        //        List<FileInfo> listFiles = new List<FileInfo>();

        //        if (localFiles == null)
        //        {
        //            return 0;
        //        }

        //        try
        //        {
        //            foreach (string file in localFiles)
        //            {
        //                if (!File.Exists(file))
        //                {
        //                    Log4NetUtil.Error(this, "UploadFiles->本地文件不存在:" + file);
        //                    continue;
        //                }
        //                listFiles.Add(new FileInfo(file));
        //            }

        //            //远端路径校验
        //            if (string.IsNullOrEmpty(remoteDic))
        //            {
        //                remoteDic = "/";
        //            }
        //            if (!remoteDic.StartsWith("/"))
        //            {
        //                remoteDic = "/" + remoteDic;
        //            }
        //            if (!remoteDic.EndsWith("/"))
        //            {
        //                remoteDic += "/";
        //            }

        //            if (Connect())
        //            {
        //                if (listFiles.Count > 0)
        //                {
        //                    count = ftpClient.UploadFiles(listFiles, remoteDic, FtpExists.Overwrite, true);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log4NetUtil.Error(this, "UploadFiles->上传文件 异常:" + ex.ToString());
        //        }
        //        finally
        //        {
        //            DisConnect();
        //        }

        //        return count;
        //    }
        //    #endregion

        //    #region 下载单文件
        //    /// <summary>
        //    /// 下载单文件
        //    /// </summary>
        //    /// <param name="localDic">本地目录(@"D:\test")</param>
        //    /// <param name="remotePath">远程路径("/test/abc.txt")</param>
        //    /// <returns></returns>
        //    public bool DownloadFile(string localDic, string remotePath)
        //    {
        //        bool boolResult = false;
        //        string strFileName = string.Empty;

        //        try
        //        {
        //            //本地目录不存在，则自动创建
        //            if (!Directory.Exists(localDic))
        //            {
        //                Directory.CreateDirectory(localDic);
        //            }
        //            //取下载文件的文件名
        //            strFileName = Path.GetFileName(remotePath);
        //            //拼接本地路径
        //            localDic = Path.Combine(localDic, strFileName);

        //            if (Connect())
        //            {
        //                boolResult = ftpClient.DownloadFile(localDic, remotePath, FtpLocalExists.Overwrite);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log4NetUtil.Error(this, "DownloadFile->下载文件 异常:" + ex.ToString() + "|*|remotePath:" + remotePath);
        //        }
        //        finally
        //        {
        //            DisConnect();
        //        }

        //        return boolResult;
        //    }
        //    #endregion

        //    #region 下载多文件
        //    /// <summary>
        //    /// 下载多文件
        //    /// </summary>
        //    /// <param name="localDic">本地目录(@"D:\test")</param>
        //    /// <param name="remotePath">远程路径列表</param>
        //    /// <returns></returns>
        //    public int DownloadFiles(string localDic, IEnumerable<string> remoteFiles)
        //    {
        //        int count = 0;
        //        if (remoteFiles == null)
        //        {
        //            return 0;
        //        }

        //        try
        //        {
        //            //本地目录不存在，则自动创建
        //            if (!Directory.Exists(localDic))
        //            {
        //                Directory.CreateDirectory(localDic);
        //            }

        //            if (Connect())
        //            {
        //                count = ftpClient.DownloadFiles(localDic, remoteFiles, FtpLocalExists.Overwrite);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log4NetUtil.Error(this, "DownloadFiles->下载文件 异常:" + ex.ToString());
        //        }
        //        finally
        //        {
        //            DisConnect();
        //        }

        //        return count;
        //    }
        //    #endregion

        //    #region 判断文件是否存在
        //    /// <summary>
        //    /// 判断文件是否存在
        //    /// </summary>
        //    /// <param name="remotePath">远程路径("/test/abc.txt")</param>
        //    /// <returns></returns>
        //    public bool IsFileExists(string remotePath)
        //    {
        //        bool boolResult = false;

        //        try
        //        {
        //            if (Connect())
        //            {
        //                boolResult = ftpClient.FileExists(remotePath);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log4NetUtil.Error(this, "IsFileExists->判断文件是否存在 异常:" + ex.ToString() + "|*|remotePath:" + remotePath);
        //        }
        //        finally
        //        {
        //            DisConnect();
        //        }

        //        return boolResult;
        //    }
        //    #endregion

        //    #region 判断目录是否存在
        //    /// <summary>
        //    /// 判断目录是否存在
        //    /// </summary>
        //    /// <param name="remotePath">远程路径("/test")</param>
        //    /// <returns></returns>
        //    public bool IsDirExists(string remotePath)
        //    {
        //        bool boolResult = false;

        //        try
        //        {
        //            if (Connect())
        //            {
        //                boolResult = ftpClient.DirectoryExists(remotePath);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log4NetUtil.Error(this, "IsDirExists->判断目录是否存在 异常:" + ex.ToString() + "|*|remotePath:" + remotePath);
        //        }
        //        finally
        //        {
        //            DisConnect();
        //        }

        //        return boolResult;
        //    }
        //    #endregion

        //    #region 新建目录
        //    /// <summary>
        //    /// 新建目录
        //    /// </summary>
        //    /// <param name="remoteDic">远程目录("/test")</param>
        //    /// <returns></returns>
        //    public bool MakeDir(string remoteDic)
        //    {
        //        bool boolResult = false;

        //        try
        //        {
        //            if (Connect())
        //            {
        //                ftpClient.CreateDirectory(remoteDic);

        //                boolResult = true;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log4NetUtil.Error(this, "MakeDir->新建目录 异常:" + ex.ToString() + "|*|remoteDic:" + remoteDic);
        //        }
        //        finally
        //        {
        //            DisConnect();
        //        }

        //        return boolResult;
        //    }
        //    #endregion

        //    #region 清理
        //    /// <summary>
        //    /// 清理
        //    /// </summary>
        //    public void Clean()
        //    {
        //        //断开FTP
        //        DisConnect();

        //        if (ftpClient != null)
        //        {
        //            ftpClient.Dispose();
        //        }
        //    }
        //    #endregion
        //}
    }
}
