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

namespace YAF.DotNetNuke
{
    #region Using

    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using global::DotNetNuke.Common;
    using global::DotNetNuke.Entities.Modules;
    using global::DotNetNuke.Services.Localization;

    using global::DotNetNuke.Services.Scheduling;

    using YAF.DotNetNuke.Components.Utils;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// User Importer.
    /// </summary>
    public partial class YafDnnModuleImport : PortalModuleBase
    {
        #region Constants and Fields

        /// <summary>
        /// The type full name.
        /// </summary>
        private const string TypeFullName = "YAF.DotNetNuke.YafDnnImportScheduler, YAF.DotNetNuke.Module";

        /// <summary>
        /// The board id.
        /// </summary>
        private int boardId;

        #endregion

        #region Methods

        /// <summary>
        /// The add scheduler click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void AddSchedulerClick(object sender, EventArgs e)
        {
            var btn = sender.ToType<LinkButton>();

            switch (btn.CommandArgument)
            {
                case "add":
                    this.InstallScheduleClient();
                    btn.CommandArgument = "delete";
                    btn.Text = Localization.GetString("DeleteShedulerText", this.LocalResourceFile);
                    break;
                case "delete":
                    RemoveScheduleClient(GetIdOfScheduleClient(TypeFullName));
                    btn.CommandArgument = "add";
                    btn.Text = Localization.GetString("InstallSheduler.Text", this.LocalResourceFile);
                    break;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += this.DotNetNukeModuleImport_Load;

            this.btnImportUsers.Click += this.ImportClick;
            this.Close.Click += this.CloseClick;
            this.btnAddScheduler.Click += this.AddSchedulerClick;

            base.OnInit(e);
        }

        /// <summary>
        /// The get id of schedule client.
        /// </summary>
        /// <param name="typeFullName">
        /// The type full name.
        /// </param>
        /// <returns>
        /// Returns the id of schedule client.
        /// </returns>
        private static int GetIdOfScheduleClient(string typeFullName)
        {
            // get array list of schedule items
            ArrayList schduleItems = SchedulingProvider.Instance().GetSchedule();

            // find schedule item with matching TypeFullName
            foreach (object item in
                schduleItems.Cast<object>().Where(item => ((ScheduleItem)item).TypeFullName == typeFullName))
            {
                return ((ScheduleItem)item).ScheduleID;
            }

            return -1;
        }

        /// <summary>
        /// The remove schedule client.
        /// </summary>
        /// <param name="scheduleId">The schedule id.</param>
        private static void RemoveScheduleClient(int scheduleId)
        {
            // get the item by id
            var item = SchedulingProvider.Instance().GetSchedule(scheduleId);

            // tell the provider to remove the item
            SchedulingProvider.Instance().DeleteSchedule(item);
        }

        /// <summary>
        /// The close click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CloseClick(object sender, EventArgs e)
        {
            this.Response.Redirect(Globals.NavigateURL(), true);
        }

        /// <summary>
        /// Handles the Load event of the DotNetNukeModuleImport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DotNetNukeModuleImport_Load(object sender, EventArgs e)
        {
            this.btnImportUsers.Text = Localization.GetString("ImportNow.Text", this.LocalResourceFile);
            this.Close.Text = Localization.GetString("Close.Text", this.LocalResourceFile);
            this.btnAddScheduler.Text = Localization.GetString("InstallSheduler.Text", this.LocalResourceFile);

            try
            {
                this.boardId = this.Settings["forumboardid"].ToType<int>();
            }
            catch (Exception)
            {
                this.boardId = 1;
            }

            if (this.IsPostBack || GetIdOfScheduleClient(TypeFullName) <= 0)
            {
                return;
            }

            var importFile = "{0}App_Data/YafImports.xml".FormatWith(HttpRuntime.AppDomainAppPath);

            var dsSettings = new DataSet();

            try
            {
                dsSettings.ReadXml(importFile);
            }
            catch (Exception)
            {
                var file = new FileStream(importFile, FileMode.Create);
                var sw = new StreamWriter(file);

                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<YafImports>");
                sw.WriteLine("<Import PortalId=\"{0}\" BoardId=\"{1}\"/>".FormatWith(this.PortalId, this.boardId));
                sw.WriteLine("</YafImports>");

                sw.Close();
                file.Close();
            }

            var updateXml = false;

            foreach (DataRow oRow in dsSettings.Tables[0].Rows)
            {
                int iPortal = oRow["PortalId"].ToType<int>();
                int iBoard = oRow["BoardId"].ToType<int>();

                if (iPortal.Equals(this.PortalId) && iBoard.Equals(this.boardId))
                {
                    updateXml = false;
                    break;
                }

                updateXml = true;
            }

            if (updateXml)
            {
                DataRow dr = dsSettings.Tables["Import"].NewRow();

                dr["PortalId"] = this.PortalId.ToString();
                dr["BoardId"] = this.boardId.ToString();

                dsSettings.Tables[0].Rows.Add(dr);

                dsSettings.WriteXml(importFile);
            }

            this.btnAddScheduler.CommandArgument = "delete";
            this.btnAddScheduler.Text = Localization.GetString("DeleteSheduler.Text", this.LocalResourceFile);
        }

        /// <summary>
        /// Import/Update Users and Sync Roles
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ImportClick(object sender, EventArgs e)
        {
            string info;

            UserImporter.ImportUsers(this.boardId, this.PortalSettings.PortalId, this.PortalSettings.GUID, out info);

            this.lInfo.Text = info;
        }

        /// <summary>
        /// The install schedule client.
        /// </summary>
        private void InstallScheduleClient()
        {
            var item = new ScheduleItem
                {
                    FriendlyName = "YAF DotNetNuke User Importer",
                    TypeFullName = TypeFullName,
                    TimeLapse = 1,
                    TimeLapseMeasurement = "d",
                    RetryTimeLapse = 1,
                    RetryTimeLapseMeasurement = "d",
                    RetainHistoryNum = 5,
                    AttachToEvent = "None",
                    CatchUpEnabled = true,
                    Enabled = true,
                    ObjectDependencies = string.Empty,
                    Servers = string.Empty
                };

            // add item
            SchedulingProvider.Instance().AddSchedule(item);

            var dsSettings = new DataSet();

            var filePath = "{0}App_Data/YafImports.xml".FormatWith(HttpRuntime.AppDomainAppPath);

            try
            {
                dsSettings.ReadXml(filePath);
            }
            catch (Exception)
            {
                var file = new FileStream(filePath, FileMode.Create);
                var sw = new StreamWriter(file);

                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<YafImports>");
                sw.WriteLine("<Import PortalId=\"{0}\" BoardId=\"{1}\"/>".FormatWith(this.PortalId, this.boardId));
                sw.WriteLine("</YafImports>");

                sw.Close();
                file.Close();
            }
        }

        #endregion
    }
}