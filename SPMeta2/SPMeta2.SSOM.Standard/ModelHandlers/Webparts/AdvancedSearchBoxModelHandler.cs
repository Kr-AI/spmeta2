using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using Microsoft.SharePoint.Portal.WebControls;
using Microsoft.SharePoint.Publishing.WebControls;

using Microsoft.SharePoint.WebPartPages;
using Microsoft.Office.Server.Search.WebControls;
using Microsoft.Office.Server.WebControls;

using SPMeta2.Definitions;
using SPMeta2.Definitions.Webparts;
using SPMeta2.Services;
using SPMeta2.SSOM.ModelHandlers;
using SPMeta2.SSOM.ModelHosts;
using SPMeta2.Standard.Definitions.Webparts;
using SPMeta2.Utils;
using WebPart = System.Web.UI.WebControls.WebParts.WebPart;

namespace SPMeta2.SSOM.Standard.ModelHandlers.Webparts
{
    public class AdvancedSearchBoxModelHandler : WebPartModelHandler
    {
        #region properties

        public override Type TargetType
        {
            get { return typeof(AdvancedSearchBoxDefinition); }
        }

        #endregion

        #region methods

        protected override void OnBeforeDeployModel(WebpartPageModelHost host, WebPartDefinition webpartModel)
        {
            var typedModel = webpartModel.WithAssertAndCast<AdvancedSearchBoxDefinition>("webpartModel", value => value.RequireNotNull());
            typedModel.WebpartType = typeof(AdvancedSearchBox).AssemblyQualifiedName;
        }

        protected override void ProcessWebpartProperties(WebPart webpartInstance, WebPartDefinition webpartModel)
        {
            base.ProcessWebpartProperties(webpartInstance, webpartModel);

            var typedWebpart = webpartInstance.WithAssertAndCast<AdvancedSearchBox>("webpartInstance", value => value.RequireNotNull());
            var definition = webpartModel.WithAssertAndCast<AdvancedSearchBoxDefinition>("webpartModel", value => value.RequireNotNull());

            if (!string.IsNullOrEmpty(definition.AndQueryTextBoxLabelText))
                typedWebpart.AndQueryTextBoxLabelText = definition.AndQueryTextBoxLabelText;

            if (!string.IsNullOrEmpty(definition.DisplayGroup))
                typedWebpart.DisplayGroup = definition.DisplayGroup;

            if (!string.IsNullOrEmpty(definition.LanguagesLabelText))
                typedWebpart.LanguagesLabelText = definition.LanguagesLabelText;

            if (!string.IsNullOrEmpty(definition.NotQueryTextBoxLabelText))
                typedWebpart.NotQueryTextBoxLabelText = definition.NotQueryTextBoxLabelText;

            if (!string.IsNullOrEmpty(definition.OrQueryTextBoxLabelText))
                typedWebpart.OrQueryTextBoxLabelText = definition.OrQueryTextBoxLabelText;

            if (!string.IsNullOrEmpty(definition.PhraseQueryTextBoxLabelText))
                typedWebpart.PhraseQueryTextBoxLabelText = definition.PhraseQueryTextBoxLabelText;

            if (!string.IsNullOrEmpty(definition.AdvancedSearchBoxProperties))
                typedWebpart.Properties = definition.AdvancedSearchBoxProperties;

            if (!string.IsNullOrEmpty(definition.PropertiesSectionLabelText))
                typedWebpart.PropertiesSectionLabelText = definition.PropertiesSectionLabelText;

            if (!string.IsNullOrEmpty(definition.ResultTypeLabelText))
                typedWebpart.ResultTypeLabelText = definition.ResultTypeLabelText;

            if (!string.IsNullOrEmpty(definition.ScopeLabelText))
                typedWebpart.ScopeLabelText = definition.ScopeLabelText;

            if (!string.IsNullOrEmpty(definition.ScopeSectionLabelText))
                typedWebpart.ScopeSectionLabelText = definition.ScopeSectionLabelText;

            if (!string.IsNullOrEmpty(definition.SearchResultPageURL))
                typedWebpart.SearchResultPageURL = definition.SearchResultPageURL;

            if (definition.ShowAndQueryTextBox.HasValue)
                typedWebpart.ShowAndQueryTextBox = definition.ShowAndQueryTextBox.Value;

            if (definition.ShowLanguageOptions.HasValue)
                typedWebpart.ShowLanguageOptions = definition.ShowLanguageOptions.Value;

            if (definition.ShowNotQueryTextBox.HasValue)
                typedWebpart.ShowNotQueryTextBox = definition.ShowNotQueryTextBox.Value;

            if (definition.ShowOrQueryTextBox.HasValue)
                typedWebpart.ShowOrQueryTextBox = definition.ShowOrQueryTextBox.Value;

            if (definition.ShowPhraseQueryTextBox.HasValue)
                typedWebpart.ShowPhraseQueryTextBox = definition.ShowPhraseQueryTextBox.Value;

            if (definition.ShowPropertiesSection.HasValue)
                typedWebpart.ShowPropertiesSection = definition.ShowPropertiesSection.Value;

            if (definition.ShowResultTypePicker.HasValue)
                typedWebpart.ShowResultTypePicker = definition.ShowResultTypePicker.Value;

            if (definition.ShowScopes.HasValue)
                typedWebpart.ShowScopes = definition.ShowScopes.Value;

            if (!string.IsNullOrEmpty(definition.TextQuerySectionLabelText))
                typedWebpart.TextQuerySectionLabelText = definition.TextQuerySectionLabelText;
        }

        #endregion
    }
}
