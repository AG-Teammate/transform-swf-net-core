﻿using System;
using System.Collections.Generic;
using System.IO;

/*
 * ImageInfo.java
 *
 * Version 1.9
 *
 * A Java class to determine image width, height and color depth for
 * a number of image file formats.
 *
 * Written by Marco Schmidt
 *
 * Contributed to the Public Domain.
 */

namespace com.flagstone.transform.util.image
{

    /// <summary>
    /// Get file format, image resolution, number of bits per pixel and optionally
    /// number of images, comments and physical resolution from JPEG, GIF, BMP, PCX,
    /// PNG, IFF, RAS, PBM, PGM, PPM and PSD files (or input streams).
    /// <para>
    /// Use the class like this:
    /// 
    /// <pre>
    /// ImageInfo ii = new ImageInfo();
    /// ii.setInput(in); // in can be InputStream or RandomAccessFile
    /// ii.setDetermineImageNumber(true); // default is false
    /// ii.setCollectComments(true); // default is false
    /// if (!ii.check()) {
    ///     System.err.println(&quot;Not a supported image file format.&quot;);
    ///     return;
    /// }
    /// System.out.println(ii.getFormatName() + &quot;, &quot; + ii.getMimeType()
    ///         + &quot;, &quot;
    ///         + ii.getWidth() + &quot; x &quot; + ii.getHeight()
    ///         + &quot; pixels, &quot;
    ///         + ii.getBitsPerPixel() + &quot; bits per pixel, &quot;
    ///         + ii.getNumberOfImages()
    ///         + &quot; image(s), &quot; + ii.getNumberOfComments()
    ///         + &quot; comment(s).&quot;);
    /// // there are other properties, check out the API documentation
    /// </pre>
    /// 
    /// You can also use this class as a command line program. Call it with a number
    /// of image file names and URLs as parameters:
    /// 
    /// <pre>
    ///   java ImageInfo *.jpg *.png *.gif http://somesite.tld/image.jpg
    /// </pre>
    /// 
    /// or call it without parameters and pipe data to it:
    /// 
    /// <pre>
    ///   java ImageInfo &lt; image.jpg
    /// </pre>
    /// </para>
    /// <para>
    /// Known limitations:
    /// <ul>
    /// <li>When the determination of the number of images is turned off, GIF bits
    /// per pixel are only read from the global header. For some GIFs, local palettes
    /// change this to a typically larger value. To be certain to get the correct
    /// color depth, call setDetermineImageNumber(true) before calling check(). The
    /// complete scan over the GIF file will take additional time.</li>
    /// <li>Transparency information is not included in the bits per pixel count.
    /// Actually, it was my decision not to include those bits, so it's a feature!
    /// ;-)</li>
    /// </ul>
    /// </para>
    /// <para>
    /// Requirements:
    /// <ul>
    /// <li>Java 1.1 or higher</li>
    /// </ul>
    /// </para>
    /// <para>
    /// The latest version can be found at <a
    /// href="http://schmidt.devlib.org/image-info/"
    /// >http://schmidt.devlib.org/image-info/</a>.
    /// </para>
    /// <para>
    /// Written by Marco Schmidt.
    /// </para>
    /// <para>
    /// This class is contributed to the Public Domain. Use it at your own risk.
    /// </para>
    /// <para>
    /// <a name="history">History</a>:
    /// <ul>
    /// <li><strong>2001-08-24</strong> Initial version.</li>
    /// <li><strong>2001-10-13</strong> Added support for the file formats BMP and
    /// PCX.</li>
    /// <li><strong>2001-10-16</strong> Fixed bug in read(int[], int, int) that
    /// returned
    /// <li><strong>2002-01-22</strong> Added support for file formats Amiga IFF and
    /// Sun Raster (RAS).</li>
    /// <li><strong>2002-01-24</strong> Added support for file formats Portable
    /// Bitmap / Graymap / Pixmap (PBM, PGM, PPM) and Adobe Photoshop (PSD). Added
    /// new method getMimeType() to return the MIME type associated with a particular
    /// file format.</li>
    /// <li><strong>2002-03-15</strong> Added support to recognize number of images
    /// in file. Only works with GIF. Use setDetermineImageNumber with
    /// <code>true</code> as argument to identify animated GIFs (
    /// <seealso cref="#getNumberOfImages()"/> will return a value larger than <code>1</code>).
    /// </li>
    /// <li><strong>2002-04-10</strong> Fixed a bug in the feature 'determine number
    /// of images in animated GIF' introduced with version 1.1. Thanks to Marcelo P.
    /// Lima for sending in the bug report. Released as 1.1.1.</li>
    /// <li><strong>2002-04-18</strong> Added <seealso cref="#setCollectComments(boolean)"/>.
    /// That new method lets the user specify whether textual comments are to be
    /// stored in an internal list when encountered in an input image file / stream.
    /// Added two methods to return the physical width and height of the image in
    /// dpi: <seealso cref="#getPhysicalWidthDpi()"/> and <seealso cref="#getPhysicalHeightDpi()"/>. If
    /// the physical resolution could not be retrieved, these methods return
    /// <code>-1</code>.</li>
    /// <li><strong>2002-04-23</strong> Added support for the new properties physical
    /// resolution and comments for some formats. Released as 1.2.</li>
    /// <li><strong>2002-06-17</strong> Added support for SWF, sent in by Michael
    /// Aird. Changed checkJpeg() so that other APP markers than APP0 will not lead
    /// to a failure anymore. Released as 1.3.</li>
    /// <li><strong>2003-07-28</strong> Bug fix - skip method now takes return values
    /// into consideration. Less bytes than necessary may have been skipped, leading
    /// to flaws in the retrieved information in some cases. Thanks to Bernard
    /// Bernstein for pointing that out. Released as 1.4.</li>
    /// <li><strong>2004-02-29</strong> Added support for recognizing progressive
    /// JPEG and interlaced PNG and GIF. A new method <seealso cref="#isProgressive()"/>
    /// returns whether ImageInfo has found that the storage type is progressive (or
    /// interlaced). Thanks to Joe Germuska for suggesting the feature. Bug fix: BMP
    /// physical resolution is now correctly determined. Released as 1.5.</li>
    /// <li><strong>2004-11-30</strong> Bug fix: recognizing progressive GIFs
    /// (interlaced in GIF terminology) did not work (thanks to Franz Jeitler for
    /// pointing this out). Now it should work, but only if the number of images is
    /// determined. This is because information on interlacing is stored in a local
    /// image header. In theory, different images could be stored interlaced and
    /// non-interlaced in one file. However, I think that's unlikely. Right now, the
    /// last image in the GIF file that is examined by ImageInfo is used for the
    /// "progressive" status.</li>
    /// <li><strong>2005-01-02</strong> Some code clean up (unused methods and
    /// variables commented out, missing javadoc comments, etc.). Thanks to George
    /// Sexton for a long list. Removed usage of Boolean.toString because it's a Java
    /// 1.4+ feature (thanks to Gregor Dupont). Changed delimiter character in
    /// compact output from semicolon to tabulator (for better integration with
    /// cut(1) and other Unix tools). Added some points to the <a
    /// href="http://schmidt.devlib.org/image-info/index.html#knownissues">'Known
    /// issues' section of the website</a>. Released as 1.6.</li>
    /// <li><strong>2005-07-26</strong> Removed code to identify Flash (SWF) files.
    /// Has repeatedly led to problems and support requests, and I don't know the
    /// format and don't have the time and interest to fix it myself. I repeatedly
    /// included fixes by others which didn't work for some people. I give up on SWF.
    /// Please do not contact me about it anymore. Set package of ImageInfo class to
    /// org.devlib.schmidt.imageinfo (a package was repeatedly requested by some
    /// users). Released as 1.7.</li>
    /// <li><strong>2006-02-23</strong> Removed Flash helper methods which weren't
    /// used elsewhere. Updated skip method which tries "read" whenever "skip(Bytes)"
    /// returns a result of 0. The old method didn't work with certain input stream
    /// types on truncated data streams. Thanks to Martin Leidig for reporting this
    /// and sending in code. Released as 1.8.</li>
    /// </li>
    /// <li><strong>2006-11-13</strong> Removed check that made ImageInfo report JPEG
    /// APPx markers smaller than 14 bytes as files in unknown format. Such JPEGs
    /// seem to be generated by Google's Picasa application. First reported with fix
    /// by Karl von Randow. Released as 1.9.</li>
    /// </ul>
    /// 
    /// @author Marco Schmidt
    /// </para>
    /// </summary>
    

    public sealed class ImageInfo
    {
        /// <summary>
        /// Return value of <seealso cref="#getFormat()"/> for JPEG streams. ImageInfo can
        /// extract physical resolution and comments from JPEGs (only from APP0
        /// headers). Only one image can be stored in a file. It is determined
        /// whether the JPEG stream is progressive (see <seealso cref="#isProgressive()"/>).
        /// </summary>
        public const int FORMAT_JPEG = 0;

        /// <summary>
        /// Return value of <seealso cref="#getFormat()"/> for GIF streams. ImageInfo can
        /// extract comments from GIFs and count the number of images (GIFs with more
        /// than one image are animations). It is determined whether the GIF stream
        /// is interlaced (see <seealso cref="#isProgressive()"/>).
        /// </summary>
        public const int FORMAT_GIF = 1;

        /// <summary>
        /// Return value of <seealso cref="#getFormat()"/> for PNG streams. PNG only supports
        /// one image per file. Both physical resolution and comments can be stored
        /// with PNG, but ImageInfo is currently not able to extract those. It is
        /// determined whether the PNG stream is interlaced (see
        /// <seealso cref="#isProgressive()"/>).
        /// </summary>
        public const int FORMAT_PNG = 2;

        /// <summary>
        /// Return value of <seealso cref="#getFormat()"/> for BMP streams. BMP only supports
        /// one image per file. BMP does not allow for comments. The physical
        /// resolution can be stored.
        /// </summary>
        public const int FORMAT_BMP = 3;

        /// <summary>
        /// Return value of <seealso cref="#getFormat()"/> for PCX streams. PCX does not allow
        /// for comments or more than one image per file. However, the physical
        /// resolution can be stored.
        /// </summary>
        public const int FORMAT_PCX = 4;

        /// <summary>
        /// Return value of <seealso cref="#getFormat()"/> for IFF streams.
        /// </summary>
        public const int FORMAT_IFF = 5;

        /// <summary>
        /// Return value of <seealso cref="#getFormat()"/> for RAS streams. Sun Raster allows
        /// for one image per file only and is not able to store physical resolution
        /// or comments.
        /// </summary>
        public const int FORMAT_RAS = 6;

        /// <summary>
        /// Return value of <seealso cref="#getFormat()"/> for PBM streams. </summary>
        public const int FORMAT_PBM = 7;

        /// <summary>
        /// Return value of <seealso cref="#getFormat()"/> for PGM streams. </summary>
        public const int FORMAT_PGM = 8;

        /// <summary>
        /// Return value of <seealso cref="#getFormat()"/> for PPM streams. </summary>
        public const int FORMAT_PPM = 9;

        /// <summary>
        /// Return value of <seealso cref="#getFormat()"/> for PSD streams. </summary>
        public const int FORMAT_PSD = 10;

        /*
		 * public static final int COLOR_TYPE_UNKNOWN = -1; public static final int
		 * COLOR_TYPE_TRUECOLOR_RGB = 0; public static final int COLOR_TYPE_PALETTED
		 * = 1; public static final int COLOR_TYPE_GRAYSCALE= 2; public static final
		 * int COLOR_TYPE_BLACK_AND_WHITE = 3;
		 */

        /// <summary>
        /// The names of all supported file formats. The FORMAT_xyz int constants can
        /// be used as index values for this array.
        /// </summary>
        private static readonly string[] FORMAT_NAMES = { "JPEG", "GIF", "PNG", "BMP", "PCX", "IFF", "RAS", "PBM", "PGM", "PPM", "PSD" };

        /// <summary>
        /// The names of the MIME types for all supported file formats. The
        /// FORMAT_xyz int constants can be used as index values for this array.
        /// </summary>
        private static readonly string[] MIME_TYPE_STRINGS = { "image/jpeg", "image/gif", "image/png", "image/bmp", "image/pcx", "image/iff", "image/ras", "image/x-portable-bitmap", "image/x-portable-graymap", "image/x-portable-pixmap", "image/psd" };

        private ImageEncoding imageFormat;
        private int width;
        private int height;
        private int bitsPerPixel;
        // private int colorType = COLOR_TYPE_UNKNOWN;
        private bool progressive;
        private int format;
        private Stream @in;
        private BinaryReader din;
        private bool collectComments = true;
        private List<string> comments;
        //    private boolean determineNumberOfImages;
        private int numberOfImages;
        private int physicalHeightDpi;
        private int physicalWidthDpi;



        private void addComment(string s)
        {
            if (comments == null)
            {
                comments = new List<string>();
            }
            comments.Add(s);
        }

        /// <summary>
        /// Call this method after you have provided an input stream or file using
        /// <seealso cref="#setInput(InputStream)"/> or <seealso cref="#setInput(DataInput)"/>. If true
        /// is returned, the file format was known and information on the file's
        /// content can be retrieved using the various getXyz methods.
        /// </summary>
        /// <returns> if information could be retrieved from input </returns>
        public bool check()
        {
            format = -1;
            width = -1;
            height = -1;
            bitsPerPixel = -1;
            numberOfImages = 1;
            physicalHeightDpi = -1;
            physicalWidthDpi = -1;
            comments = null;
            try
            {


                int b1 = read() & 0xff;


                int b2 = read() & 0xff;
                if ((b1 == 0x47) && (b2 == 0x49))
                {
                    return false; //checkGif();
                }
                if ((b1 == 0x89) && (b2 == 0x50))
                {
                    return checkPng();
                }
                if ((b1 == 0xff) && (b2 == 0xd8))
                {
                    return checkJpeg();
                }
                if ((b1 == 0x42) && (b2 == 0x4d))
                {
                    return checkBmp();
                    /* } else if ((b1 == 0x0a) && (b2 < 0x06)) {
					return false; //checkPcx();
				} else if ((b1 == 0x46) && (b2 == 0x4f)) {
					return false; //checkIff();
				} else if ((b1 == 0x59) && (b2 == 0xa6)) {
					return false; //checkRas();
				} else if ((b1 == 0x50) && (b2 >= 0x31) && (b2 <= 0x36)) {
					return false; //checkPnm(b2 - '0');
				} else if ((b1 == 0x38) && (b2 == 0x42)) {
					return false; //checkPsd();  */
                }
                return false;
            }


            catch (IOException)
            {
                return false;
            }
        }



        private bool checkBmp()
        {


            byte[] a = new byte[44];
            if (read(a) != a.Length)
            {
                return false;
            }
            width = getIntLittleEndian(a, 16);
            height = getIntLittleEndian(a, 20);
            if ((width < 1) || (height < 1))
            {
                return false;
            }
            bitsPerPixel = getShortLittleEndian(a, 26);
            if ((bitsPerPixel != 1) && (bitsPerPixel != 4) && (bitsPerPixel != 8) && (bitsPerPixel != 16) && (bitsPerPixel != 24) && (bitsPerPixel != 32))
            {
                return false;
            }


            int x = (int)(getIntLittleEndian(a, 36) * 0.0254);
            if (x > 0)
            {
                PhysicalWidthDpi = x;
            }


            int y = (int)(getIntLittleEndian(a, 40) * 0.0254);
            if (y > 0)
            {
                PhysicalHeightDpi = y;
            }
            format = FORMAT_BMP;
            imageFormat = ImageEncoding.BMP;
            return true;
        }

        //    private boolean checkGif() throws IOException {
        //        final byte[] gifmagic87a = { 0x46, 0x38, 0x37, 0x61 };
        //        final byte[] gifmagic89a = { 0x46, 0x38, 0x39, 0x61 };
        //        final byte[] a = new byte[11]; // 4 from the GIF signature + 7 from
        //                                        // the global
        //        // header
        //        if (read(a) != 11) {
        //            return false;
        //        }
        //        if ((!equals(a, 0, gifmagic89a, 0, 4))
        //                && (!equals(a, 0, gifmagic87a, 0, 4))) {
        //            return false;
        //        }
        //        format = FORMAT_GIF;
        //        imageFormat = ImageEncoding.GIF;
        //        width = getShortLittleEndian(a, 4);
        //        height = getShortLittleEndian(a, 6);
        //        int flags = a[8] & 0xff;
        //        bitsPerPixel = ((flags >> 4) & 0x07) + 1;
        //        // progressive = (flags & 0x02) != 0;
        //        if (!determineNumberOfImages) {
        //            return true;
        //        }
        //        // skip global color palette
        //        if ((flags & 0x80) != 0) {
        //            final int tableSize = (1 << ((flags & 7) + 1)) * 3;
        //            skip(tableSize);
        //        }
        //        numberOfImages = 0;
        //        int blockType;
        //        int n;
        //        do {
        //            blockType = read();
        //            switch (blockType) {
        //            case (0x2c): // image separator
        //                if (read(a, 0, 9) != 9) {
        //                    return false;
        //                }
        //                flags = a[8] & 0xff;
        //                progressive = (flags & 0x40) != 0;
        //                /*
        //                 * int locWidth = getShortLittleEndian(a, 4); int locHeight =
        //                 * getShortLittleEndian(a, 6); System.out.println("LOCAL: " +
        //                 * locWidth + " x " + locHeight);
        //                 */
        //                final int localBitsPerPixel = (flags & 0x07) + 1;
        //                if (localBitsPerPixel > bitsPerPixel) {
        //                    bitsPerPixel = localBitsPerPixel;
        //                }
        //                if ((flags & 0x80) != 0) {
        //                    skip((1 << localBitsPerPixel) * 3);
        //                }
        //                skip(1); // initial code length
        //                do {
        //                    n = read();
        //                    if (n > 0) {
        //                        skip(n);
        //                    } else if (n == -1) {
        //                        return false;
        //                    }
        //                } while (n > 0);
        //                numberOfImages++;
        //                break;
        //            case (0x21): // extension
        //                final int extensionType = read();
        //                if (collectComments && (extensionType == 0xfe)) {
        //                    final StringBuilder sb = new StringBuilder();
        //                    do {
        //                        n = read();
        //                        if (n == -1) {
        //                            return false;
        //                        }
        //                        if (n > 0) {
        //                            for (int i = 0; i < n; i++) {
        //                                final int ch = read();
        //                                if (ch == -1) {
        //                                    return false;
        //                                }
        //                                sb.append((char) ch);
        //                            }
        //                        }
        //                    } while (n > 0);
        //                } else {
        //                    do {
        //                        n = read();
        //                        if (n > 0) {
        //                            skip(n);
        //                        } else if (n == -1) {
        //                            return false;
        //                        }
        //                    } while (n > 0);
        //                }
        //                break;
        //            case (0x3b): // end of file
        //                break;
        //            default:
        //                return false;
        //            }
        //        } while (blockType != 0x3b);
        //        return true;
        //    }

        //    private boolean checkIff() throws IOException {
        //        final byte[] a = new byte[10];
        //        // read remaining 2 bytes of file id, 4 bytes file size
        //        // and 4 bytes IFF subformat
        //        if (read(a, 0, 10) != 10) {
        //            return false;
        //        }
        //        final byte[] iffrm = { 0x52, 0x4d };
        //        if (!equals(a, 0, iffrm, 0, 2)) {
        //            return false;
        //        }
        //        final int type = getIntBigEndian(a, 6);
        //        if ((type != 0x494c424d) && // type must be ILBM...
        //                (type != 0x50424d20)) { // ...or PBM
        //            return false;
        //        }
        //        // loop chunks to find BMHD chunk
        //        do {
        //            if (read(a, 0, 8) != 8) {
        //                return false;
        //            }
        //            final int chunkId = getIntBigEndian(a, 0);
        //            int size = getIntBigEndian(a, 4);
        //            if ((size & 1) == 1) {
        //                size++;
        //            }
        //            if (chunkId == 0x424d4844) { // BMHD chunk
        //                if (read(a, 0, 9) != 9) {
        //                    return false;
        //                }
        //                format = FORMAT_IFF;
        //                imageFormat = ImageEncoding.IFF;
        //                width = getShortBigEndian(a, 0);
        //                height = getShortBigEndian(a, 2);
        //                bitsPerPixel = a[8] & 0xff;
        //                return ((width > 0) && (height > 0)
        //                        && (bitsPerPixel > 0) && (bitsPerPixel < 33));
        //            } else {
        //                skip(size);
        //            }
        //        } while (true);
        //    }



        private bool checkJpeg()
        {


            byte[] data = new byte[12];
            while (true)
            {
                if (read(data, 0, 4) != 4)
                {
                    return false;
                }


                int marker = getShortBigEndian(data, 0);
                int size = getShortBigEndian(data, 2);
                if ((marker & 0xff00) != 0xff00)
                {
                    return false; // not a valid marker
                }
                if (marker == 0xffe0)
                { // APPx
                    if (size < 14)
                    {
                        // not an APPx header as we know it, skip
                        skip(size - 2);
                        continue;
                    }
                    if (read(data, 0, 12) != 12)
                    {
                        return false;
                    }


                    byte[] appoId = { 0x4a, 0x46, 0x49, 0x46, 0x00 };
                    if (Equals(appoId, 0, data, 0, 5))
                    {
                        // System.out.println("data 7=" + data[7]);
                        if (data[7] == 1)
                        {
                            PhysicalWidthDpi = getShortBigEndian(data, 8);
                            PhysicalHeightDpi = getShortBigEndian(data, 10);
                        }
                        else if (data[7] == 2)
                        {


                            int x = getShortBigEndian(data, 8);


                            int y = getShortBigEndian(data, 10);
                            PhysicalWidthDpi = (int)(x * 2.54f);
                            PhysicalHeightDpi = (int)(y * 2.54f);
                        }
                    }
                    skip(size - 14);
                }
                else if (collectComments && (size > 2) && (marker == 0xfffe))
                { // comment
                    size -= 2;


                    byte[] chars = new byte[size];
                    if (read(chars, 0, size) != size)
                    {
                        return false;
                    }
                    string comment = StringHelperClass.NewString(chars, "iso-8859-1");
                    comment = comment.Trim();
                    addComment(comment);
                }
                else if ((marker >= 0xffc0) && (marker <= 0xffcf) && (marker != 0xffc4) && (marker != 0xffc8))
                {
                    if (read(data, 0, 6) != 6)
                    {
                        return false;
                    }
                    format = FORMAT_JPEG;
                    imageFormat = ImageEncoding.JPEG;
                    bitsPerPixel = (data[0] & 0xff) * (data[5] & 0xff);
                    progressive = (marker == 0xffc2) || (marker == 0xffc6) || (marker == 0xffca) || (marker == 0xffce);
                    width = getShortBigEndian(data, 3);
                    height = getShortBigEndian(data, 1);
                    return true;
                }
                else
                {
                    skip(size - 2);
                }
            }
        }

        //    private boolean checkPcx() throws IOException {
        //        final byte[] a = new byte[64];
        //        if (read(a) != a.length) {
        //            return false;
        //        }
        //        if (a[0] != 1) { // encoding, 1=RLE is only valid value
        //            return false;
        //        }
        //        // width / height
        //        final int x1 = getShortLittleEndian(a, 2);
        //        final int y1 = getShortLittleEndian(a, 4);
        //        final int x2 = getShortLittleEndian(a, 6);
        //        final int y2 = getShortLittleEndian(a, 8);
        //        if ((x1 < 0) || (x2 < x1) || (y1 < 0) || (y2 < y1)) {
        //            return false;
        //        }
        //        width = x2 - x1 + 1;
        //        height = y2 - y1 + 1;
        //        // color depth
        //        final int bits = a[1];
        //        final int planes = a[63];
        //        if ((planes == 1)
        //                && ((bits == 1) || (bits == 2)
        //                  || (bits == 4) || (bits == 8))) {
        //            // paletted
        //            bitsPerPixel = bits;
        //        } else if ((planes == 3) && (bits == 8)) {
        //            // RGB truecolor
        //            bitsPerPixel = 24;
        //        } else {
        //            return false;
        //        }
        //        setPhysicalWidthDpi(getShortLittleEndian(a, 10));
        //        setPhysicalHeightDpi(getShortLittleEndian(a, 10));
        //        format = FORMAT_PCX;
        //        imageFormat = ImageEncoding.PCX;
        //        return true;
        //    }



        private bool checkPng()
        {


            byte[] pngmagic = { 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };


            byte[] a = new byte[27];
            if (read(a) != 27)
            {
                return false;
            }
            if (!Equals(a, 0, pngmagic, 0, 6))
            {
                return false;
            }
            format = FORMAT_PNG;
            imageFormat = ImageEncoding.PNG;
            width = getIntBigEndian(a, 14);
            height = getIntBigEndian(a, 18);
            bitsPerPixel = a[22] & 0xff;


            int colorType = a[23] & 0xff;
            if ((colorType == 2) || (colorType == 6))
            {
                bitsPerPixel *= 3;
            }
            progressive = (a[26] & 0xff) != 0;
            return true;
        }

        //    private boolean checkPnm(final int id) throws IOException {
        //        if ((id < 1) || (id > 6)) {
        //            return false;
        //        }
        //        final int[] pnmformats = { FORMAT_PBM, FORMAT_PGM, FORMAT_PPM };
        //        format = pnmformats[(id - 1) % 3];
        //        boolean hasPixelResolution = false;
        //        String s;
        //        while (true) {
        //            s = readLine();
        //            if (s != null) {
        //                s = s.trim();
        //            }
        //            if ((s == null) || (s.length() < 1)) {
        //                continue;
        //            }
        //            if (s.charAt(0) == '#') { // comment
        //                if (collectComments && (s.length() > 1)) {
        //                    addComment(s.substring(1));
        //                }
        //                continue;
        //            }
        //            if (!hasPixelResolution) { // split "343 966" into width=343,
        //                // height=966
        //                int spaceIndex = s.indexOf(' ');
        //                if (spaceIndex == -1) {
        //                    return false;
        //                }
        //                final String widthString = s.substring(0, spaceIndex);
        //                spaceIndex = s.lastIndexOf(' ');
        //                if (spaceIndex == -1) {
        //                    return false;
        //                }
        //                final String heightString = s.substring(spaceIndex + 1);
        //                try {
        //                    width = Integer.parseInt(widthString);
        //                    height = Integer.parseInt(heightString);
        //                } catch (final NumberFormatException nfe) {
        //                    return false;
        //                }
        //                if ((width < 1) || (height < 1)) {
        //                    return false;
        //                }
        //                if (format == FORMAT_PBM) {
        //                    bitsPerPixel = 1;
        //                    return true;
        //                }
        //                hasPixelResolution = true;
        //            } else {
        //                int maxSample;
        //                try {
        //                    maxSample = Integer.parseInt(s);
        //                } catch (final NumberFormatException nfe) {
        //                    return false;
        //                }
        //                if (maxSample < 0) {
        //                    return false;
        //                }
        //                for (int i = 0; i < 25; i++) {
        //                    if (maxSample < (1 << (i + 1))) {
        //                        bitsPerPixel = i + 1;
        //                        if (format == FORMAT_PPM) {
        //                            bitsPerPixel *= 3;
        //                        }
        //                        return true;
        //                    }
        //                }
        //                return false;
        //            }
        //        }
        //    }

        //    private boolean checkPsd() throws IOException {
        //        final byte[] a = new byte[24];
        //        if (read(a) != a.length) {
        //            return false;
        //        }
        //        final byte[] psdmagic = { 0x50, 0x53 };
        //        if (!equals(a, 0, psdmagic, 0, 2)) {
        //            return false;
        //        }
        //        format = FORMAT_PSD;
        //        imageFormat = ImageEncoding.PSD;
        //        width = getIntBigEndian(a, 16);
        //        height = getIntBigEndian(a, 12);
        //        final int channels = getShortBigEndian(a, 10);
        //        final int depth = getShortBigEndian(a, 20);
        //        bitsPerPixel = channels * depth;
        //        return ((width > 0) && (height > 0)
        //                && (bitsPerPixel > 0) && (bitsPerPixel <= 64));
        //    }

        //    private boolean checkRas() throws IOException {
        //        final byte[] a = new byte[14];
        //        if (read(a) != a.length) {
        //            return false;
        //        }
        //        final byte[] rasmagic = { 0x6a, (byte) 0x95 };
        //        if (!equals(a, 0, rasmagic, 0, 2)) {
        //            return false;
        //        }
        //        format = FORMAT_RAS;
        //        imageFormat = ImageEncoding.RAS;
        //        width = getIntBigEndian(a, 2);
        //        height = getIntBigEndian(a, 6);
        //        bitsPerPixel = getIntBigEndian(a, 10);
        //        return ((width > 0) && (height > 0)
        //                && (bitsPerPixel > 0) && (bitsPerPixel <= 24));
        //    }

        /// <summary>
        /// Run over String list, return false iff at least one of the arguments
        /// equals <code>-c</code>.
        /// </summary>
        /// <param name="args">
        ///            string list to check </param>


        private static bool determineVerbosity(string[] args)
        {
            if ((args != null) && (args.Length > 0))
            {
                foreach (String arg in args)
                {
                    if ("-c".Equals(arg))
                    {
                        return false;
                    }
                }
            }
            return true;
        }



        private static bool Equals(byte[] a1, int offs1, byte[] a2, int offs2, int num)
        {
            int index1 = offs1;
            int index2 = offs2;
            int count = num;
            while (count-- > 0)
            {
                if (a1[index1++] != a2[index2++])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// If <seealso cref="#check()"/> was successful, returns the image's number of bits
        /// per pixel. Does not include transparency information like the alpha
        /// channel.
        /// </summary>
        /// <returns> number of bits per image pixel </returns>
        public int BitsPerPixel => bitsPerPixel;

        /// <summary>
        /// Returns the index'th comment retrieved from the file.
        /// </summary>
        /// <param name="index">
        ///            int index of comment to return to the number of comments
        ///            retrieved </param>
        /// <seealso cref= #getNumberOfComments </seealso>


        public string getComment(int index)
        {
            if ((comments == null) || (index < 0) || (index >= comments.Count))
            {
                throw new ArgumentException("Not a valid comment index: " + index);
            }
            return comments[index];
        }

        /// <summary>
        /// If <seealso cref="#check()"/> was successful, returns the image format as one of
        /// the FORMAT_xyz constants from this class. Use <seealso cref="#getFormatName()"/> to
        /// get a textual description of the file format.
        /// </summary>
        /// <returns> file format as a FORMAT_xyz constant </returns>
        public int Format => format;


        public ImageEncoding ImageFormat => imageFormat;

        /// <summary>
        /// If <seealso cref="#check()"/> was successful, returns the image format's name. Use
        /// <seealso cref="#getFormat()"/> to get a unique number.
        /// </summary>
        /// <returns> file format name </returns>
        public string FormatName
        {
            get
            {
                if ((format >= 0) && (format < FORMAT_NAMES.Length))
                {
                    return FORMAT_NAMES[format];
                }
                return "?";
            }
        }

        /// <summary>
        /// If <seealso cref="#check()"/> was successful, returns one the image's vertical
        /// resolution in pixels.
        /// </summary>
        /// <returns> image height in pixels </returns>
        public int Height => height;



        private static int getIntBigEndian(byte[] a, int offs)
        {
            return (a[offs] & 0xff) << 24 | (a[offs + 1] & 0xff) << 16 | (a[offs + 2] & 0xff) << 8 | a[offs + 3] & 0xff;
        }



        private static int getIntLittleEndian(byte[] a, int offs)
        {
            return (a[offs + 3] & 0xff) << 24 | (a[offs + 2] & 0xff) << 16 | (a[offs + 1] & 0xff) << 8 | a[offs] & 0xff;
        }

        /// <summary>
        /// If <seealso cref="#check()"/> was successful, returns a String with the MIME type
        /// of the format.
        /// </summary>
        /// <returns> MIME type, e.g. <code>image/jpeg</code> </returns>
        public string MimeType
        {
            get
            {
                if ((format >= 0) && (format < MIME_TYPE_STRINGS.Length))
                {
                    if ((format == FORMAT_JPEG) && progressive)
                    {
                        return "image/pjpeg";
                    }
                    return MIME_TYPE_STRINGS[format];
                }
                return null;
            }
        }

        /// <summary>
        /// If <seealso cref="#check()"/> was successful and
        /// <seealso cref="#setCollectComments(boolean)"/> was called with <code>true</code> as
        /// argument, returns the number of comments retrieved from the input image
        /// stream / file. Any number &gt;= 0 and smaller than this number of
        /// comments is then a valid argument for the <seealso cref="#getComment(int)"/>
        /// method.
        /// </summary>
        /// <returns> number of comments retrieved from input image </returns>
        public int NumberOfComments
        {
            get
            {
                if (comments == null)
                {
                    return 0;
                }
                return comments.Count;
            }
        }

        /// <summary>
        /// Returns the number of images in the examined file. Assumes that
        /// <code>setDetermineImageNumber(true);</code> was called before a
        /// successful call to <seealso cref="#check()"/>. This value can currently be only
        /// different from <code>1</code> for GIF images.
        /// </summary>
        /// <returns> number of images in file </returns>
        public int NumberOfImages => numberOfImages;

        /// <summary>
        /// Returns the physical height of this image in dots per inch (dpi). Assumes
        /// that <seealso cref="#check()"/> was successful. Returns <code>-1</code> on failure.
        /// </summary>
        /// <returns> physical height (in dpi) </returns>
        /// <seealso cref= #getPhysicalWidthDpi() </seealso>
        /// <seealso cref= #getPhysicalHeightInch() </seealso>
        public int PhysicalHeightDpi
        {
            get => physicalHeightDpi;
            set => physicalWidthDpi = value;
        }

        /// <summary>
        /// If <seealso cref="#check()"/> was successful, returns the physical width of this
        /// image in dpi (dots per inch) or -1 if no value could be found.
        /// </summary>
        /// <returns> physical height (in dpi) </returns>
        /// <seealso cref= #getPhysicalHeightDpi() </seealso>
        /// <seealso cref= #getPhysicalWidthDpi() </seealso>
        /// <seealso cref= #getPhysicalWidthInch() </seealso>
        public float PhysicalHeightInch
        {
            get
            {


                int h = Height;


                int ph = PhysicalHeightDpi;
                if ((h > 0) && (ph > 0))
                {
                    return h / ((float)ph);
                }
                return -1.0f;
            }
        }

        /// <summary>
        /// If <seealso cref="#check()"/> was successful, returns the physical width of this
        /// image in dpi (dots per inch) or -1 if no value could be found.
        /// </summary>
        /// <returns> physical width (in dpi) </returns>
        /// <seealso cref= #getPhysicalHeightDpi() </seealso>
        /// <seealso cref= #getPhysicalWidthInch() </seealso>
        /// <seealso cref= #getPhysicalHeightInch() </seealso>
        public int PhysicalWidthDpi
        {
            get => physicalWidthDpi;
            set => physicalHeightDpi = value;
        }

        /// <summary>
        /// Returns the physical width of an image in inches, or <code>-1.0f</code>
        /// if width information is not available. Assumes that <seealso cref="#check"/> has
        /// been called successfully.
        /// </summary>
        /// <returns> physical width in inches or <code>-1.0f</code> on failure </returns>
        /// <seealso cref= #getPhysicalWidthDpi </seealso>
        /// <seealso cref= #getPhysicalHeightInch </seealso>
        public float PhysicalWidthInch
        {
            get
            {


                int w = Width;


                int pw = PhysicalWidthDpi;
                if ((w > 0) && (pw > 0))
                {
                    return w / ((float)pw);
                }
                return -1.0f;
            }
        }



        private static int getShortBigEndian(byte[] a, int offs)
        {
            return (a[offs] & 0xff) << 8 | (a[offs + 1] & 0xff);
        }



        private static int getShortLittleEndian(byte[] a, int offs)
        {
            return (a[offs] & 0xff) | (a[offs + 1] & 0xff) << 8;
        }

        /// <summary>
        /// If <seealso cref="#check()"/> was successful, returns one the image's horizontal
        /// resolution in pixels.
        /// </summary>
        /// <returns> image width in pixels </returns>
        public int Width => width;

        /// <summary>
        /// Returns whether the image is stored in a progressive (also called:
        /// interlaced) way.
        /// </summary>
        /// <returns> true for progressive/interlaced, false otherwise </returns>
        public bool Progressive => progressive;

        /// <summary>
        /// To use this class as a command line application, give it either some file
        /// names as parameters (information on them will be printed to standard
        /// output, one line per file) or call it with no parameters. It will then
        /// check data given to it via standard input.
        /// </summary>
        /// <param name="args">
        ///            the program arguments which must be file names </param>


        //		public static void Main(string[] args)
        //		{


        //			ImageInfo imageInfo = new ImageInfo();
        //	//        imageInfo.setDetermineImageNumber(true);


        //			bool verbose = determineVerbosity(args);
        //			if (args.Length == 0)
        //			{
        //				run(null, System.in, imageInfo, verbose);
        //			}
        //			else
        //			{
        //				int index = 0;
        //				while (index < args.Length)
        //				{
        //					System.IO.Stream @in = null;
        //					try
        //					{


        //						string name = args[index++];
        //						//System.out.print(name + ";");
        //						if (name.StartsWith("http://", StringComparison.Ordinal))
        //						{
        //							@in = (new URL(name)).openConnection().InputStream;
        //						}
        //						else
        //						{
        //							@in = new System.IO.FileStream(name, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
        //						}
        //						run(name, @in, imageInfo, verbose);
        //						@in.Close();
        //					}


        //					catch (va.io.IOException)
        //					{
        //						//System.out.println(e);
        //						try
        //						{
        //							if (@in != null)
        //							{
        //								@in.Close();
        //							}
        //						}


        //						catch (IOException ee)
        //						{
        //							Console.WriteLine(ee.ToString());
        //							Console.Write(ee.StackTrace);
        //						}
        //					}
        //				}
        //			}
        //		}

        //    private static void print(final String sourceName, final ImageInfo ii,
        //            final boolean verbose) {
        //        if (verbose) {
        //            printVerbose(sourceName, ii);
        //        } else {
        //            printCompact(sourceName, ii);
        //        }
        //    }

        //    private static void printCompact(final String sourceName,
        //            final ImageInfo imageInfo) {
        //        final String SEP = "\t";
        //        System.out.println(sourceName + SEP + imageInfo.getFormatName() + SEP
        //                + imageInfo.getMimeType() + SEP + imageInfo.getWidth() + SEP
        //                + imageInfo.getHeight() + SEP + imageInfo.getBitsPerPixel()
        //                + SEP + imageInfo.getNumberOfImages() + SEP
        //                + imageInfo.getPhysicalWidthDpi() + SEP
        //                + imageInfo.getPhysicalHeightDpi() + SEP
        //                + imageInfo.getPhysicalWidthInch() + SEP
        //                + imageInfo.getPhysicalHeightInch() + SEP
        //                + imageInfo.isProgressive());
        //    }

        //    private static void printLine(final int indentLevels, final String text,
        //            final float value, final float minValidValue) {
        //        if (value < minValidValue) {
        //            return;
        //        }
        //        printLine(indentLevels, text, Float.toString(value));
        //    }

        //    private static void printLine(final int indentLevels, final String text,
        //            final int value, final int minValidValue) {
        //        if (value >= minValidValue) {
        //            printLine(indentLevels, text, Integer.toString(value));
        //        }
        //    }

        //    private static void printLine(int indentLevels, final String text,
        //            final String value) {
        //        if ((value == null) || (value.length() == 0)) {
        //            return;
        //        }
        //        while (indentLevels-- > 0) {
        //            System.out.print("\t");
        //        }
        //        if ((text != null) && (text.length() > 0)) {
        //            System.out.print(text);
        //            System.out.print(" ");
        //        }
        //        System.out.println(value);
        //    }

        //    private static void printVerbose(final String
        //    sourceName, final ImageInfo ii) {
        //        printLine(0, null, sourceName);
        //        printLine(1, "File format: ", ii.getFormatName());
        //        printLine(1, "MIME type: ", ii.getMimeType());
        //        printLine(1, "Width (pixels): ", ii.getWidth(), 1);
        //        printLine(1, "Height (pixels): ", ii.getHeight(), 1);
        //        printLine(1, "Bits per pixel: ", ii.getBitsPerPixel(), 1);
        //        printLine(1, "Progressive: ", ii.isProgressive() ? "yes" : "no");
        //        printLine(1, "Number of images: ", ii.getNumberOfImages(), 1);
        //        printLine(1, "Physical width (dpi): ", ii.getPhysicalWidthDpi(), 1);
        //        printLine(1, "Physical height (dpi): ", ii.getPhysicalHeightDpi(), 1);
        //        printLine(1, "Physical width (inches): ", ii.getPhysicalWidthInch(),
        //                1.0f);
        //        printLine(1, "Physical height (inches): ", ii.getPhysicalHeightInch(),
        //                1.0f);
        //        final int numComments = ii.getNumberOfComments();
        //        printLine(1, "Number of textual comments: ", numComments, 1);
        //        if (numComments > 0) {
        //            for (int i = 0; i < numComments; i++) {
        //                printLine(2, null, ii.getComment(i));
        //            }
        //        }
        //    }



        private int read()
        {
            if (@in != null)
            {
                return @in.ReadByte();
            }
            return din.ReadByte();
        }




        private int read(byte[] a)
        {
            if (@in != null)
            {
                return @in.Read(a, 0, a.Length);
            }
            din.Read(a, 0, a.Length);
            return a.Length;
        }




        private int read(byte[] a, int offset, int num)
        {
            if (@in != null)
            {
                return @in.Read(a, offset, num);
            }
            din.Read(a, offset, num);
            return num;
        }

        //    private String readLine() throws IOException {
        //        return readLine(new StringBuilder());
        //    }

        //    private String readLine(final StringBuilder sb) throws IOException {
        //        boolean finished;
        //        do {
        //            final int value = read();
        //            finished = ((value == -1) || (value == 10));
        //            if (!finished) {
        //                sb.append((char) value);
        //            }
        //        } while (!finished);
        //        return sb.toString();
        //    }



        private static void run(string sourceName, Stream @in, ImageInfo imageInfo, bool verbose)
        {
            imageInfo.Input = @in;
            //        imageInfo.setDetermineImageNumber(true);
            imageInfo.CollectComments = verbose;
            //        if (imageInfo.check()) {
            //            print(sourceName, imageInfo, verbose);
            //        }
        }

        /// <summary>
        /// Specify whether textual comments are supposed to be extracted from input.
        /// Default is <code>false</code>. If enabled, comments will be added to an
        /// internal list.
        /// </summary>
        /// <param name="newValue">
        ///            if <code>true</code>, this class will read comments </param>
        /// <seealso cref= #getNumberOfComments </seealso>
        /// <seealso cref= #getComment </seealso>


        public bool CollectComments
        {
            set => collectComments = value;
        }

        /// <summary>
        /// Specify whether the number of images in a file is to be determined -
        /// default is <code>false</code>. This is a special option because some file
        /// formats require running over the entire file to find out the number of
        /// images, a rather time-consuming task. Not all file formats support more
        /// than one image. If this method is called with <code>true</code> as
        /// argument, the actual number of images can be queried via
        /// <seealso cref="#getNumberOfImages()"/> after a successful call to <seealso cref="#check()"/>.
        /// </summary>
        /// <param name="newValue">
        ///            will the number of images be determined? </param>
        /// <seealso cref= #getNumberOfImages </seealso>
        //    public void setDetermineImageNumber(final boolean newValue) {
        //        determineNumberOfImages = newValue;
        //    }

        /// <summary>
        /// Set the input stream to the argument stream (or file). Note that
        /// <seealso cref="java.io.RandomAccessFile"/> implements <seealso cref="java.io.DataInput"/>.
        /// </summary>
        /// <param name="dataInput">
        ///            the input stream to read from </param>


        public BinaryReader InputReader
        {
            set
            {
                din = value;
                @in = null;
            }
        }

        /// <summary>
        /// Set the input stream to the argument stream (or file).
        /// </summary>
        /// <param name="inputStream">
        ///            the input stream to read from </param>


        public Stream Input
        {
            set
            {
                @in = value;
                din = null;
            }
        }






        private void skip(int num)
        {
            int count = num;
            while (count > 0)
            {
                long result;
                if (@in != null)
                {
                    result = @in.Seek(num, SeekOrigin.Current);
                }
                else
                {
                    //result = din.(num);
                    //AG not sure how to do this
                    result = 0;
                }
                if (result > 0)
                {
                    count -= (int)result;
                }
                else
                {
                    if (@in != null)
                    {
                        result = @in.ReadByte();
                    }
                    else
                    {
                        result = din.ReadByte();
                    }
                    if (result == -1)
                    {
                        throw new IOException("Premature end of input.");
                    }
                    count--;
                }
            }
        }
    }


}