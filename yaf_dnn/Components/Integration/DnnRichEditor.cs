﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Editors
{
  #region Using

    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using global::DotNetNuke.Modules.HTMLEditorProvider;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// Adds Support for the DNN Editors
  ///   Code provided by Balbes
  ///   "http://forum.yetanotherforum.net/yaf_postst8907_DotNetNuke-HTMLEditorProvider-integration-UPDATED-to-YAF-1-9-4.aspx"
  /// </summary>
  public class DnnRichEditor : RichClassEditor
  {
    #region Constants and Fields

    /// <summary>
    /// The _editor loaded.
    /// </summary>
    private readonly bool _editorLoaded;

    /// <summary>
    /// The _editor.
    /// </summary>
    private HtmlEditorProvider _editor;

    /// <summary>
    /// The _style sheet.
    /// </summary>
    private string _styleSheet;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DnnRichEditor"/> class.
    /// </summary>
    public DnnRichEditor()
    {
      this._styleSheet = string.Empty;
      this._editorLoaded = this.InitDnnEditor();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether Active.
    /// </summary>
    public override bool Active
    {
      get
      {
        return this._editorLoaded;
      }
    }

    /// <summary>
    /// Gets Description.
    /// </summary>
    public override string Description
    {
      get
      {
        return "DotNetNuke Text Editor (HTML)";
      }
    }

    /// <summary>
    /// Gets ModuleId.
    /// </summary>
    public override string ModuleId
    {
      get
      {
        // backward compatibility...
        return "9";
      }
    }

    /// <summary>
    /// Gets or sets StyleSheet.
    /// </summary>
    public override string StyleSheet
    {
      get
      {
        return this._styleSheet;
      }

      set
      {
        this._styleSheet = value;
      }
    }

    /// <summary>
    /// Gets or sets Text.
    /// </summary>
    public override string Text
    {
      get
      {
        return !this._editorLoaded ? string.Empty : this._editor.Text;
      }

      set
      {
        if (!this._editorLoaded)
        {
          return;
        }

        this._editor.Text = value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether UsesBBCode.
    /// </summary>
    public override bool UsesBBCode
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether UsesHTML.
    /// </summary>
    public override bool UsesHTML
    {
      get
      {
        return true;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the Load event of the Editor control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected virtual void Editor_Load(object sender, EventArgs e)
    {
      if (!this._editorLoaded || !this._editor.Visible)
      {
        return;
      }

      this._editor.Height = Unit.Pixel(400);
      this._editor.Width = Unit.Percentage(100); // Change Me

      this.Controls.Add(this._editor.HtmlEditorControl);

      this.RegisterSmilieyScript();
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
      if (!this._editorLoaded)
      {
        return;
      }

      this._editor.ControlID = "yafDnnRichEditor";
      this._editor.Initialize();
      this.Load += this.Editor_Load;
      base.OnInit(e);
    }

    /// <summary>
    /// The register smiley script.
    /// </summary>
    protected virtual void RegisterSmilieyScript()
    {
      Type editorType = this._editor.GetType();
      Control editor = this.FindControl(this._editor.ControlID);
      if (editor == null)
      {
        return;
      }

      switch (editorType.ToString())
      {
        case "Telerik.DNN.Providers.RadEditorProvider":
          this.Page.ClientScript.RegisterClientScriptBlock(
            this.Page.GetType(), 
            "insertsmiley",
            @"<script type='text/javascript'>
               function insertsmiley(code,img){{\nvar editor = $find('{0}');editor.pasteHtml('[img]' + img + '[/img]');\n}}\n
               function insertAttachment(id,url){{\nvar editor = $find('{0}');editor.pasteHtml('[attach]' + id + '[/attach]');\n}}\n
              </script>".FormatWith(editor.ClientID));
          break;
        case "DotNetNuke.HtmlEditor.FckHtmlEditorProvider.FckHtmlEditorProvider":
          this.Page.ClientScript.RegisterClientScriptBlock(
            this.Page.GetType(), 
            "insertsmiley",
            @"<script language=""javascript"" type=""text/javascript"">\n
               function insertsmiley(code,img) {{\nvar oEditor = FCKeditorAPI.GetInstance('{0}');\n oEditor.InsertHtml( '[img]' + img + '[/img]' );\n}}\n
               function insertAttachment(id, url) {{\nvar oEditor = FCKeditorAPI.GetInstance('{0}');\n oEditor.InsertHtml( '[attach]' + id + '[/attach]' );\n}}\n
              </script>\n".FormatWith(editor.ClientID.Replace("$", "_")));
          break;
        case "WatchersNET.CKEditor.CKHtmlEditorProvider":
              this.Page.ClientScript.RegisterClientScriptBlock(
                  this.Page.GetType(),
                  "insertsmiley",
                  @"<script language=""javascript"" type=""text/javascript\"">\n
                       function insertsmiley(code,img) {{\nvar ckEditor = CKEDITOR.instances.{0}; ckEditor.insertHtml( '[img]' + img + '[/img]' );\n}}\n
                       function insertAttachment(id,url) {{\nvar ckEditor = CKEDITOR.instances.{0}; ckEditor.insertHtml( '[attach]' + id + '[/attach]' );\n}}\n
                    </script>\n"
                      .FormatWith(editor.ClientID));
          break;
        case "DotNetNuke.HtmlEditor.TelerikEditorProvider.EditorProvider":
          this.Page.ClientScript.RegisterClientScriptBlock(
            this.Page.GetType(), 
            "insertsmiley",
            @"<script type='text/javascript'>
                     function insertsmiley(code,img){{\nvar editor = $find('{0}');editor.pasteHtml('[img]' + img + '[/img]');\n}}\n
                     function insertAttachment(id,url){{\nvar editor = $find('{0}');editor.pasteHtml('[attach]' + id + '[/attach]');\n}}\n
               </script>".FormatWith(editor.ClientID));
          break;
      }
    }

    /// <summary>
    /// Initialize the DNN editor.
    /// </summary>
    /// <returns>
    /// Returns if the DNN Editor was initialized.
    /// </returns>
    private bool InitDnnEditor()
    {
      if (!Config.IsDotNetNuke)
      {
        return false;
      }

      try
      {
        this._editor = HtmlEditorProvider.Instance();
        return true;
      }
      catch (Exception ex)
      {
          YafContext.Current.Get<ILogger>().Error(ex, "Error in the DNN RichEditor");
      }

      return false;
    }

    #endregion
  }
}