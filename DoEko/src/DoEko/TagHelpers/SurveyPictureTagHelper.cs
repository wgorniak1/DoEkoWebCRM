using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Html;

namespace DoEko.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("survey-picture")]
    public class SurveyPictureTagHelper : TagHelper
    {
        [HtmlAttributeName("picture-id")]
        public string Id { get; set; }

        [HtmlAttributeName("picture-title")]
        public string Title { get; set; }

        [HtmlAttributeName("picture-link")]
        public string Link { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = string.Empty;

            if (string.IsNullOrEmpty(Link))
            {
                
                output.Content.AppendHtml("<label class=\"control-label\">" + Title + "</label>");
                output.Content.AppendHtml("<a class=\"photo-link\" style=\"cursor:pointer\" name=\"" + Id + "\">");
                output.Content.AppendHtml("<img src=\"\" alt=\"Dodaj zdjęcie\" style=\"width:100%; min-height:100px; max-height:100px;\" class=\"img-thumbnail wg-image-placeholder\" />");
                output.Content.AppendHtml("</a>");
                output.Content.AppendHtml("<form action=\"\" method=\"post\" enctype=\"multipart/form-data\" hidden>");
                output.Content.AppendHtml("<input type=\"file\" id=\"" + Id + "\" name=\"" + Id + "\" class=\"photo-input\" accept=\"image/*\" capture>");
                output.Content.AppendHtml("<input type=\"submit\" value=\"Upload\">");
                output.Content.AppendHtml("</form>");
                output.Content.AppendHtml("<button class=\"btn btn-sm btn-default photo-delete\" data-photo-name=\"" + Id + "\" hidden style=\"display:none; margin-top:5px;\">");
                output.Content.AppendHtml("<span class=\"glyphicon glyphicon-remove-circle text-danger\"></span>");
                output.Content.AppendHtml("</button>");
            }
            else
            {
                output.Content.AppendHtml("<label class=\"control-label\">" + Title + "</label>");
                output.Content.AppendHtml("<a href=\"" + Link + "\" target=\"_blank\" class=\"photo-link\" style=\"cursor:pointer\" name=\"" + Id + "\">");
                output.Content.AppendHtml("<img src=\"" + Link + "\" alt=\"" + Title + "\" style=\"width:100%; min-height:100px; max-height:100px;\" class=\"img-thumbnail wg-image-placeholder\" />");
                output.Content.AppendHtml("</a>");
                output.Content.AppendHtml("<form action=\"\" method=\"post\" enctype=\"multipart/form-data\" hidden>");
                output.Content.AppendHtml("<input type=\"file\" id=\"" + Id + "\" name=\"" + Id + "\" class=\"photo-input\" accept=\"image/*\" capture>");
                output.Content.AppendHtml("<input type=\"submit\" value=\"Upload\">");
                output.Content.AppendHtml("</form>");
                output.Content.AppendHtml("<button class=\"btn btn-sm btn-default photo-delete\" data-photo-name=\"" + Id + "\" style=\"margin-top:5px;\">");
                output.Content.AppendHtml("<span class=\"glyphicon glyphicon-remove-circle text-danger\"></span>");
                output.Content.AppendHtml("</button>");
            }
        }
    }
}
