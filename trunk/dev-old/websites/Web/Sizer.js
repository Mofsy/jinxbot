//=============================================================================
// System  : Sandcastle Help File Builder
// File    : TOC.js
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 04/11/2008
// Note    : Copyright 2006-2008, Eric Woodruff, All rights reserved
// Compiler: JavaScript
//
// This file contains the methods necessary to implement a simple tree view
// for the table of content with a resizable splitter and Ajax support to
// load tree nodes on demand.  It also contains the script necessary to do
// full-text searches.
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy
// of the license should be distributed with the code.  It can also be found
// at the project website: http://www.CodePlex.com/SHFB.   This notice, the
// author's name, and all copyright notices must remain intact in all
// applications, documentation, and source files.
//
// Version     Date     Who  Comments
// ============================================================================
// 1.3.0.0  09/12/2006  EFW  Created the code
// 1.4.0.2  06/15/2007  EFW  Reworked to get rid of frame set and to add
//                           support for Ajax to load tree nodes on demand.
// 1.5.0.0  06/24/2007  EFW  Added full-text search capabilities
// 1.6.0.7  04/01/2008  EFW  Merged changes from Ferdinand Prantl to add a
//                           website keyword index.  Added support for "topic"
//                           query string option.
//=============================================================================

// IE flag
var isIE = (navigator.userAgent.indexOf("MSIE") >= 0);

// Minimum width of the channel div
var minWidth = 100;

//============================================================================

// Initialize the tree view and resize the content
function Initialize()
{
}

// Resize the content div
function ResizeContent()
{
    if(isIE)
        maxWidth = docBody.clientWidth;
    else
        maxWidth = docBody.clientWidth - 4;

    topicContent.style.width = maxWidth - (divSizer.offsetLeft +
        divSizer.offsetWidth);
    maxWidth -= minWidth;
}

// This is called to prepare for dragging the sizer div
function OnMouseDown(event)
{
    var x;

    // Make sure the splitter is at the top of the z-index
    divSizer.style.zIndex = 5000;

    // The content is in an IFRAME which steals mouse events so
    // hide it while resizing.
    topicContent.style.display = "none";

    if(isIE)
        x = window.event.clientX + document.documentElement.scrollLeft +
            document.body.scrollLeft;
    else
        x = event.clientX + window.scrollX;

    // Save starting offset
    offset = parseInt(divSizer.style.left, 10);

    if(isNaN(offset))
        offset = 0;

    offset -= x;

    if(isIE)
    {
        document.attachEvent("onmousemove", OnMouseMove);
        document.attachEvent("onmouseup", OnMouseUp);
        window.event.cancelBubble = true;
        window.event.returnValue = false;
    }
    else
    {
        document.addEventListener("mousemove", OnMouseMove, true);
        document.addEventListener("mouseup", OnMouseUp, true);
        event.preventDefault();
    }
}

// Resize the TOC and content divs as the sizer is dragged
function OnMouseMove(event)
{
    var x, pos;

    // Get cursor position with respect to the page
    if(isIE)
        x = window.event.clientX + document.documentElement.scrollLeft +
            document.body.scrollLeft;
    else
        x = event.clientX + window.scrollX;

    left = offset + x;

    // Adjusts the width of the TOC divs
    pos = (event.clientX > maxWidth) ? maxWidth :
        (event.clientX < minWidth) ? minWidth : event.clientX;

    divTOC.style.width = divSearchResults.style.width =
        divIndexResults.style.width = divTree.style.width = pos;

    if(!isIE)
        pos -= 8;

    divNavOpts.style.width = divSearchOpts.style.width =
        divIndexOpts.style.width = pos;

    // Resize the content div to fit in the remaining space
    ResizeContent();
}

// Finish the drag operation when the mouse button is released
function OnMouseUp(event)
{
    if(isIE)
    {
        document.detachEvent("onmousemove", OnMouseMove);
        document.detachEvent("onmouseup", OnMouseUp);
    }
    else
    {
        document.removeEventListener("mousemove", OnMouseMove, true);
        document.removeEventListener("mouseup", OnMouseUp, true);
    }

    // Show the content div again
    topicContent.style.display = "inline";
}

