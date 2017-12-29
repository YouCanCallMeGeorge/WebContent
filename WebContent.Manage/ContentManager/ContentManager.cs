﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebContent.Manage.ContentClasses;
using WebContent.Manage.HelperClasses;
using WebContent.Manage.Interfaces;

namespace WebContent.Manage.ContentManager
{
    public class ContentManager : IContentManager
    {
        private IContentRepository repository;

        //--------------------------------------
        //--------------------------------------
        public ContentManager(IContentRepository repositoryArg)
        {
            repository = repositoryArg;
        }



        //--------------------------------------
        //--------------------------------------
        public BlogEntry BlogEntryCreate(string title, string summary, string content)
        {
            ContentTransfer contentXfer = new ContentTransfer
            {
                Content = content,
                Path = BlogEntry.PathMake(),
                Summary = summary,
                Title = title
            };

            return new BlogEntry(NodeCreate(contentXfer));
        }



        //--------------------------------------
        //--------------------------------------
        public BlogEntry BlogEntryMostRecentGet()
        {
            try
            {
                return new BlogEntry(repository.NodeTypeMostRecentLeafGet("Blog"));
            }
            catch
            {
                return null;
            }
        }



        //--------------------------------------
        //--------------------------------------
        public List<ContentLinkInfo> ContentChildLinksGet(ContentNode node)
        {
            List<ContentTransfer> childNodes = new List<ContentTransfer>();
            ContentLinkInfo linkInfo;
            List<ContentLinkInfo> linkInfoList = new List<ContentLinkInfo>();
            int segmentChild;
            string[] segments;

            // The text for each child link will be the same as its segment in the path;
            // The child segment follows the last segment in the parent path.
            segments = node.Path.Split(ContentNode.pathDividerChar);
            segmentChild = segments.Length;

            childNodes = repository.NodeChildrenGet(node.NodeId);
            foreach (ContentTransfer childNode in childNodes)
            {
                segments = childNode.Path.Split(ContentNode.pathDividerChar);
                linkInfo = new ContentLinkInfo
                {
                    Summary = childNode.Summary,
                    Text = segments[segmentChild],
                    Url = childNode.Path
                };
                linkInfoList.Add(linkInfo);
            }

            return linkInfoList;
        }



        //--------------------------------------
        //--------------------------------------
        public ContentNode ContentGetById(int id)
        {
            try
            {
                return new BlogEntry(repository.NodeGetById(id));
            }
            catch
            {
                return null;
            }
        }



        //--------------------------------------
        //--------------------------------------
        public ContentNode ContentGetByPath(string path)
        {
            try
            {
                return new BlogEntry(repository.NodeGetByPath(path));
            }
            catch
            {
                return null;
            }
        }



        //--------------------------------------
        //--------------------------------------
        public List<ContentLinkInfo> ContentPathLinksGet(ContentNode node)
        {
            ContentLinkInfo linkInfo;
            List<ContentLinkInfo> linkInfoList = new List<ContentLinkInfo>();
            string path;
            string[] segments;

            segments = node.Path.Split(ContentNode.pathDividerChar);
            path = "";
            foreach (string segment in segments)
            {
                path += segment;
                linkInfo = new ContentLinkInfo
                {
                    Summary = "",
                    Text = segment,
                    Url = path
                };
                linkInfoList.Add(linkInfo);
                path += ContentNode.pathDividerStr;
            }

            return linkInfoList;
        }



        //--------------------------------------
        //--------------------------------------
        public List<ContentLinkInfo> ContentRecentNLinksGet(string type, int num)
        {
            List<ContentLinkInfo> linkInfoList = new List<ContentLinkInfo>();
            List<ContentTransfer> xferList;
            try
            {
                xferList = repository.NodeTypeMostRecentNLeavesGet(type, num);
                foreach (ContentTransfer xfer in xferList)
                {
                    ContentLinkInfo linkInfo = new ContentLinkInfo
                    {
                        Summary = xfer.Summary,
                        Text = xfer.Title,
                        Url = xfer.Path
                    };

                    linkInfoList.Add(linkInfo);
                }
            }
            catch
            {
            }

            return linkInfoList;
        }



        //--------------------------------------
        //--------------------------------------
        public void ContentUpdate(int id, string title, string summary, string content)
        {
            ContentTransfer contentXfer = new ContentTransfer
            {
                Content = content,
                NodeId = id,
                Summary = summary,
                Title = title
            };

            repository.NodeUpdate(contentXfer);
        }



        //--------------------------------------
        //--------------------------------------
        private ContentTransfer NodeCreate(ContentTransfer nodeNew)
        {
            ContentTransfer nodeCurrent;
            int parentId;
            string pathCurrent;
            int iterSegmentCurrent;
            int iterSegmentParentCount;
            string[] segments;
            string segmentCurrent;

            // The path is trusted, having been created by the ContentManager object.
            if (repository.NodeExistsTest(nodeNew.Path))
                throw new Exception("Cannot create node with duplicate path: " + nodeNew.Path);

            // Break the path into segments.
            segments = nodeNew.Path.Split(ContentNode.pathDividerChar);

            // Walk the path to the final parent node.
            // Create missing nodes as needed.
            parentId = 0;
            pathCurrent = "";
            iterSegmentParentCount = segments.Length - 1;
            for (iterSegmentCurrent = 0; iterSegmentCurrent < iterSegmentParentCount; iterSegmentCurrent++)
            {
                // Current segment string.
                segmentCurrent = segments[iterSegmentCurrent];

                // Path to the current segment.
                pathCurrent += segmentCurrent;

                // Node at the current segment.
                nodeCurrent = repository.NodeGetByPath(pathCurrent);
                if (nodeCurrent == null)
                {
                    // A node does not exist at the current segment.
                    // Create the node.
                    nodeCurrent = new ContentTransfer
                    {
                        Path = pathCurrent,
                        Content = "",
                        Summary = "",
                        Title = segmentCurrent,
                        ParentId = parentId
                    };

                    nodeCurrent = repository.NodeCreate(nodeCurrent);
                }   // end of creating intermediate node

                // Set up for next iteration.
                pathCurrent += ContentNode.pathDividerStr;
                parentId = nodeCurrent.NodeId;
            }       // end of loop walking intermediate segments


            // Create the new node and return it, fully populated.
            nodeNew.ParentId = parentId;
            return repository.NodeCreate(nodeNew);
        }
    }
}

