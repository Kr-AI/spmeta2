﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.Client;
using SPMeta2.CSOM.ModelHosts;
using SPMeta2.Exceptions;
using SPMeta2.Services;

namespace SPMeta2.CSOM.Services
{
    public class CSOMTokenReplacementService : TokenReplacementServiceBase
    {
        #region constructors

        static CSOMTokenReplacementService()
        {
            AllowClientContextAsTokenReplacementContext = true;
        }

        public CSOMTokenReplacementService()
        {
            SupportedTokensInternal.Clear();

            TokenProcessInfos.Add(new TokenProcessInfo
            {
                Name = "~sitecollection",
                RegEx = new Regex("~sitecollection", RegexOptions.IgnoreCase)
            });

            TokenProcessInfos.Add(new TokenProcessInfo
            {
                Name = "~site",
                RegEx = new Regex("~site", RegexOptions.IgnoreCase)
            });

            SupportedTokensInternal.AddRange(TokenProcessInfos.Select(i => new TokenInfo { Name = i.Name }));
        }

        #endregion

        #region static

        /// <summary>
        /// Restrics token resolution to only CSOM model host classes
        /// Used only by regression tests
        /// 
        /// Incorrect ~site token resolution for CSOM for the subwebs #863
        /// https://github.com/SubPointSolutions/spmeta2/issues/863
        /// </summary>
        public static bool AllowClientContextAsTokenReplacementContext { get; set; }

        #endregion

        #region classes

        protected class TokenProcessInfo
        {
            public string Name { get; set; }
            public Regex RegEx { get; set; }
        }

        #endregion

        #region properties

        private List<TokenProcessInfo> TokenProcessInfos = new List<TokenProcessInfo>();

        #endregion

        #region methods

        public override TokenReplacementResult ReplaceTokens(TokenReplacementContext context)
        {
            var result = new TokenReplacementResult
            {
                Value = context.Value
            };

            if (string.IsNullOrEmpty(result.Value))
                return result;

            foreach (var tokenInfo in TokenProcessInfos)
            {
                if (!string.IsNullOrEmpty(result.Value))
                {
                    result.Value = tokenInfo.RegEx.Replace(result.Value, ResolveToken(context, context.Context, tokenInfo.Name));

                    result.Value = result.Value.Replace(@"//", @"/");
                    result.Value = result.Value.Replace(@"\\", @"\");
                }
            }

            if (OnTokenReplaced != null)
            {
                OnTokenReplaced(this, new TokenReplacementResultEventArgs
                {
                    Result = result
                });
            }

            return result;
        }

        protected virtual string ResolveToken(TokenReplacementContext tokenContext, object contextObject, string token)
        {
            if (string.Equals(token, "~sitecollection", StringComparison.CurrentCultureIgnoreCase))
            {
                if (tokenContext.IsSiteRelativeUrl)
                    return "/";

                var site = ExtractSite(contextObject);

                if (site.ServerRelativeUrl == "/")
                    return string.Empty;

                return site.ServerRelativeUrl;
            }

            if (string.Equals(token, "~site", StringComparison.CurrentCultureIgnoreCase))
            {
                var web = ExtractWeb(contextObject);

                if (tokenContext.IsSiteRelativeUrl)
                {
                    var site = ExtractSite(contextObject);
                    return "/" + web.ServerRelativeUrl.Replace(site.ServerRelativeUrl, string.Empty);
                }

                if (web.ServerRelativeUrl == "/")
                    return string.Empty;

                return web.ServerRelativeUrl;
            }

            return token;
        }

        protected virtual Web ExtractWeb(object contextObject)
        {
            if (contextObject is ClientContext)
            {
                if (AllowClientContextAsTokenReplacementContext)
                {
                    return (contextObject as ClientContext).Web;
                }
                else
                {
                    throw new SPMeta2NotSupportedException(string.Format("contextObject of type: [{0}] is not supported", contextObject.GetType()));
                }
            }

            if (contextObject is WebModelHost)
                return (contextObject as WebModelHost).HostWeb;

            if (contextObject is SiteModelHost)
                return (contextObject as SiteModelHost).HostWeb;

            if (contextObject is CSOMModelHostBase)
                return (contextObject as CSOMModelHostBase).HostWeb;

            throw new SPMeta2NotSupportedException(string.Format("contextObject of type: [{0}] is not supported", contextObject.GetType()));
        }

        protected virtual Site ExtractSite(object contextObject)
        {
            if (contextObject is ClientContext)
            {
                if (AllowClientContextAsTokenReplacementContext)
                {
                    return (contextObject as ClientContext).Site;
                }
                else
                {
                    throw new SPMeta2NotSupportedException(string.Format("contextObject of type: [{0}] is not supported", contextObject.GetType()));
                }
            }

            if (contextObject is WebModelHost)
                return (contextObject as WebModelHost).HostSite;

            if (contextObject is SiteModelHost)
                return (contextObject as SiteModelHost).HostSite;

            if (contextObject is CSOMModelHostBase)
                return (contextObject as CSOMModelHostBase).HostSite;

            throw new SPMeta2NotSupportedException(string.Format("contextObject of type: [{0}] is not supported", contextObject.GetType()));
        }

        #endregion
    }
}
