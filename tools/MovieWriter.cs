/*
 * MovieWriter.java
 * Transform
 *
 * Copyright (c) 2009-2010 Flagstone Software Ltd. All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *  * Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 *  * Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *  * Neither the name of Flagstone Software Ltd. nor the names of its
 *    contributors may be used to endorse or promote products derived from this
 *    software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System.IO;

namespace com.flagstone.transform.tools
{


    /// <summary>
    /// MovieWriter can be used to pretty print the output from the toString()
    /// method of an object or even an entire Movie.
    /// </summary>
    

    public sealed class MovieWriter
    {
        /// <summary>
        /// Pretty print an entire Movie and write it to a file. </summary>
        /// <param name="movie"> the Movie to get the string representation of. </param>
        /// <param name="file"> the file where the formatted output will be written. </param>
        /// <exception cref="IOException"> if there is an error writing to the file. </exception>



        public void write(Movie movie, FileInfo file)
        {


            TextWriter writer = new StreamWriter(file.OpenWrite());
            write(movie, writer);
            writer.Dispose();
        }
        /// <summary>
        /// Pretty print an entire Movie. </summary>
        /// <param name="movie"> the Movie to get the string representation of. </param>
        /// <param name="writer"> the Writer formatted output will be written. </param>
        /// <exception cref="IOException"> if there is an error writing to the file. </exception>



        public void write(Movie movie, TextWriter writer)
        {
            foreach (MovieTag tag in movie.Objects)
            {
                write(tag, writer);
            }
        }
        /// <summary>
        /// Pretty print an object from a Movie. </summary>
        /// <param name="obj"> the object to get the string representation of. </param>
        /// <param name="writer"> the Writer formatted output will be written. </param>
        /// <exception cref="IOException"> if there is an error writing to the file. </exception>
        



        public void write(object obj, TextWriter writer)
        {

            int level = 0;
            bool start = false;
            bool coord = false;



            string str = obj.ToString();

            foreach (char c in str)
            {

                if (c == '{')
                {
                    writer.Write(c);
                    writer.Write('\n');
                    indent(writer, ++level);
                    start = true;
                }
                else if (c == '}')
                {
                    writer.Write(";\n");
                    indent(writer, --level);
                    writer.Write(c);
                }
                else if (c == '[')
                {
                    writer.Write(c);
                    writer.Write('\n');
                    indent(writer, ++level);
                }
                else if (c == ']')
                {
                    writer.Write('\n');
                    indent(writer, --level);
                    writer.Write(c);
                }
                else if (c == ';')
                {
                    writer.Write(c);
                    writer.Write('\n');
                    indent(writer, level);
                    start = true;
                }
                else if (c == ',')
                {
                    writer.Write(c);
                    if (!coord)
                    {
                        writer.Write('\n');
                        indent(writer, level);
                        start = true;
                    }
                }
                else if (c == '<')
                {
                    writer.Write('[');
                }
                else if (c == '>')
                {
                    writer.Write(']');
                }
                else if (c == '(')
                {
                    writer.Write(c);
                    coord = true;
                }
                else if (c == ')')
                {
                    writer.Write(c);
                    coord = false;
                }
                else if (c == '=')
                {
                    writer.Write(" - ");
                }
                else if (c == ' ')
                {
                    if (!start)
                    {
                        writer.Write(c);
                    }
                }
                else
                {
                    writer.Write(c);
                    start = false;
                }
            }
            writer.Write(",\n");
            writer.Flush();
        }

        /// <summary>
        /// Indent the text by adding tabs. </summary>
        /// <param name="writer"> the Writer where the indents will be sent. </param>
        /// <param name="level"> the number of tabs to indent. </param>
        /// <exception cref="IOException"> if an error occurs while writing. </exception>



        private void indent(TextWriter writer, int level)
        {
            for (int i = 0; i < level; i++)
            {
                writer.Write('\t');
            }
        }
    }

}