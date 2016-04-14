/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
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

namespace YAF.DotNetNuke.Components.Objects
{
    #region Using

    using System;

    #endregion

    /// <summary>
    /// Messages List
    /// </summary>
    public class Messages
    {
        #region Constants and Fields

        /// <summary>
        ///   Gets or sets Message Posted at
        /// </summary>
        public DateTime Posted { get; set; }

        /// <summary>
        ///  Gets or sets The Message Id
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        ///  Gets or sets The Topic Id
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        ///  Gets or sets The Complete Message of a Post
        /// </summary>
        public string Message { get; set; }

        #endregion
    }
}