﻿using Microsoft.SharePoint.Client;
using SPMeta2.Containers.Assertion;
using SPMeta2.CSOM.Extensions;
using SPMeta2.CSOM.ModelHandlers;
using SPMeta2.CSOM.ModelHosts;
using SPMeta2.Definitions;

using SPMeta2.Utils;

namespace SPMeta2.Regression.CSOM.Validation
{
    public class ClientWebDefinitionValidator : WebModelHandler
    {
        public override void DeployModel(object modelHost, DefinitionBase model)
        {
            var hostClientContext = ExtractHostClientContext(modelHost);

            var parentWeb = ExtractWeb(modelHost);
            var definition = model.WithAssertAndCast<WebDefinition>("model", value => value.RequireNotNull());

            var currentWebUrl = GetCurrentWebUrl(parentWeb.Context, parentWeb, definition);
            var spObject = GetExistingWeb(hostClientContext.Site, parentWeb, currentWebUrl);
            var context = spObject.Context;

            context.Load(spObject,
                            w => w.HasUniqueRoleAssignments,
                            w => w.Description,
                            w => w.Url,
                            w => w.Language,
                            w => w.WebTemplate,
                            w => w.Configuration,
                            w => w.Title,
                            w => w.Id,
                            w => w.Url
                        );

            context.ExecuteQueryWithTrace();

            var assert = ServiceFactory.AssertService
                .NewAssert(definition, spObject)
                .ShouldBeEqual(m => m.Title, o => o.Title)
                .ShouldBeEqual(m => m.LCID, o => o.GetLCID())
                .ShouldBeEqual(m => m.UseUniquePermission, o => o.HasUniqueRoleAssignments);

            if (!string.IsNullOrEmpty(definition.WebTemplate))
            {
                assert.ShouldBeEqual(m => m.WebTemplate, o => o.GetWebTemplate());
                assert.SkipProperty(m => m.CustomWebTemplate);
            }
            else
            {
                assert.SkipProperty(m => m.WebTemplate);
                assert.SkipProperty(m => m.CustomWebTemplate);
            }

            if (!string.IsNullOrEmpty(definition.Description))
                assert.ShouldBeEqual(m => m.Description, o => o.Description);
            else
                assert.SkipProperty(m => m.Description, "Description is null or empty. Skipping.");

            if (!string.IsNullOrEmpty(definition.Description))
                assert.ShouldBeEqual(m => m.Description, o => o.Description);
            else
                assert.SkipProperty(m => m.Description, "Description is null or empty. Skipping.");


            assert.ShouldBeEqual((p, s, d) =>
            {
                if (!parentWeb.IsObjectPropertyInstantiated("Url"))
                {
                    parentWeb.Context.Load(parentWeb, o => o.Url);
                    parentWeb.Context.ExecuteQueryWithTrace();
                }

                var srcProp = s.GetExpressionValue(def => def.Url);
                var dstProp = d.GetExpressionValue(ct => ct.Url);

                var srcUrl = s.Url;
                var dstUrl = d.Url;

                srcUrl = UrlUtility.RemoveStartingSlash(srcUrl);

                var dstSubUrl = dstUrl.Replace(parentWeb.Url + "/", string.Empty);

                return new PropertyValidationResult
                {
                    Tag = p.Tag,
                    Src = srcProp,
                    Dst = dstProp,
                    IsValid = srcUrl == dstSubUrl
                };
            });
        }
    }

    internal static class WebExtensions
    {
        public static uint GetLCID(this Web web)
        {
            return (uint)web.Language;
        }

        public static string GetWebTemplate(this Web web)
        {
            return string.Format("{0}#{1}", web.WebTemplate, web.Configuration);
        }
    }
}
