using System;
using System.Collections.Generic;
using IOFile = System.IO.File;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BNSharp.BattleNet;
using BNSharp.MBNCSUtil;
using JinxBot.LoginService.Models;
using BNSharp.MBNCSUtil.Net;
using System.IO;
using BNSharp.MBNCSUtil.Data;

namespace JinxBot.LoginService.Controllers
{
    
    public class CheckRevisionController : Controller
    {
        private static Dictionary<string, byte> VersionBytes = new Dictionary<string, byte>() {
            { "STAR", 0xd3 },
            { "SEXP", 0xd3 },
            { "D2DV", 0x0d },
            { "D2XP", 0x0d },
            { "W2BN", 0x4f },
            { "WAR3", 0x1a },
            { "W3XP", 0x1a },
        };

        public ActionResult GetExeInfo(string productKey)
        {
            Product product = Product.GetByProductCode(productKey);

            if (product == null)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid product code.",
                    Parameters = new
                    {
                        productKey = productKey,
                    },
                });
            }

            FileData files = FileData.GetForProduct(product);
            if (files == null)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Product unsupported for CheckRevision.",
                    Parameters = new
                    {
                        productKey = productKey,
                    },
                });
            }

            string exePath = Server.MapPath("~/Content/Products/" + product.ProductCode + "/" + files.GameExe);

            string exeInfo;
            int version = CheckRevision.GetExeInfo(exePath, out exeInfo);

            byte verByte;
            if (!VersionBytes.TryGetValue(productKey, out verByte))
            {
                return Json(new
                {
                    Success = false,
                    Message = "Product not supported for CheckRevision.",
                    Parameters = new
                    {
                        productKey = productKey,
                    },
                });
            }

            return Json(new
            {
                Success = true,
                Message = "",
                Parameters = new
                {
                    productKey = productKey,
                },
                Result = new
                {
                    checksum = version,
                    exeInfo = exeInfo,
                    versionByte = verByte,
                },
            });
        }

        public ActionResult DoCheckRevision(string productKey, int mpqNumber, string challenge)
        {
            Product product = Product.GetByProductCode(productKey);
            if (product == null)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid product code.",
                    Parameters = new
                    {
                        productKey = productKey,
                        mpqNumber = mpqNumber,
                        challenge = challenge,
                    },
                });
            }

            FileData files = FileData.GetForProduct(product);
            if (files == null || string.IsNullOrWhiteSpace(files.ImageData))
            {
                return Json(new
                {
                    Success = false,
                    Message = "Product unsupported for CheckRevision.",
                    Parameters = new
                    {
                        productKey = productKey,
                        mpqNumber = mpqNumber,
                        challenge = challenge,
                    },
                });
            }

            string[] fileList = new[] {
                Server.MapPath(string.Format("~/Content/Products/{0}/{1}", productKey, files.GameExe)),
                Server.MapPath(string.Format("~/Content/Products/{0}/{1}", productKey, files.StormDll)),
                Server.MapPath(string.Format("~/Content/Products/{0}/{1}", productKey, files.GameDll)),
            };

            int crResult = CheckRevision.DoCheckRevision(challenge, fileList, mpqNumber);
            return Json(new
            {
                Success = true,
                Message = "",
                Parameters = new
                {
                    productKey = productKey,
                    mpqNumber = mpqNumber,
                    challenge = challenge,
                },
                Result = new
                {
                    value = crResult,
                },
            });
        }

        public ActionResult DoLockdownCheckRevision(string productKey, string lockdownFile, string challenge)
        {
            Product product = Product.GetByProductCode(productKey);
            if (product == null)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid product code.",
                    Parameters = new
                    {
                        productKey = productKey,
                        lockdownFile = lockdownFile,
                        challenge = challenge,
                    },
                });
            }

            FileData files = FileData.GetForProduct(product);
            if (files == null || string.IsNullOrWhiteSpace(files.ImageData))
            {
                return Json(new
                {
                    Success = false,
                    Message = "Product unsupported for Lockdown CheckRevision.",
                    Parameters = new
                    {
                        productKey = productKey,
                        lockdownFile = lockdownFile,
                        challenge = challenge,
                    },
                });
            }

            string[] fileList = new[] {
                Server.MapPath(string.Format("~/Content/Products/{0}/{1}", productKey, files.GameExe)),
                Server.MapPath(string.Format("~/Content/Products/{0}/{1}", productKey, files.StormDll)),
                Server.MapPath(string.Format("~/Content/Products/{0}/{1}", productKey, files.GameDll)),
            };
            string imageFile = Server.MapPath(string.Format("~/Content/Products/{0}/{1}", productKey, files.ImageData));

            string lockdownResource = Server.MapPath(string.Format("~/Content/Lockdown/{0}", lockdownFile.Replace(".mpq", ".dll")));
            if (!IOFile.Exists(lockdownResource))
            {
                if (!DownloadFile(productKey, lockdownFile, lockdownResource))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Unable to download requested MPQ from Battle.net.",
                        Parameters = new
                        {
                            productkey = productKey,
                            lockdownFile = lockdownFile,
                            challenge = challenge,
                        },
                    });
                }
            }

            byte[] lockdownChallenge = challenge.ToBinaryData();

            int version = 0;
            int checksum = 0;
            byte[] crValue = CheckRevision.DoLockdownCheckRevision(lockdownChallenge, fileList, lockdownResource, imageFile, ref version, ref checksum);

            return Json(new
            {
                Success = true,
                Message = "",
                Parameters = new
                {
                    productKey = productKey,
                    lockdownFile = lockdownFile,
                    challenge = challenge,
                },
                Result = new
                {
                    version = version,
                    checksum = checksum,
                    exeInformation = crValue.ToHexString(),
                },
            });
        }

        public ActionResult GetVersionByte(string productKey)
        {
            byte ver;
            if (!VersionBytes.TryGetValue(productKey, out ver))
            {
                return Json(new
                {
                    Success = false,
                    Message = "Unsupported or unrecognized product code.",
                    Parameters = new
                    {
                        productKey = productKey,
                    },
                });
            }

            return Json(new
            {
                Success = true,
                Message = "",
                Parameters = new
                {
                    productKey = productKey,
                },
                Result = ver,
            }, JsonRequestBehavior.AllowGet);
        }

        private bool DownloadFile(string productKey, string lockdownFile, string lockdownResource)
        {
            BnFtpVersion1Request req = new BnFtpVersion1Request(productKey, lockdownFile, null);
            req.Server = "useast.battle.net";
            req.LocalFileName = Server.MapPath(string.Format("~/Content/Lockdown/{0}", lockdownFile));
            req.ExecuteRequest();

            using (MpqArchive arch = MpqServices.OpenArchive(req.LocalFileName))
            {
                string dllName = lockdownFile.Replace(".mpq", ".dll");
                if (arch.ContainsFile(dllName))
                {
                    arch.SaveToPath(dllName, lockdownResource, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private new ActionResult Json(object data)
        {
            return base.Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
