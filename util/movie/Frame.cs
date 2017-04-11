using System;
using System.Collections.Generic;
using Action = com.flagstone.transform.action.Action;

/*
 * Frame.java
 * Transform
 *
 * Copyright (c) 2001-2010 Flagstone Software Ltd. All rights reserved.
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

namespace com.flagstone.transform.util.movie
{
    /// <summary>
	/// <para>
	/// The Frame class is used to provide a higher level view of a movie. Rather
	/// than viewing movies as a sequence of individual objects each representing a
	/// given data structure in the encoded Flash file, objects can be grouped
	/// together in frames which presents a more logical view of a movie and makes
	/// movie manipulation and search for specific objects easier to handle.
	/// </para>
	/// 
	/// <para>
	/// Each Frame object has the following attributes:<br/>
	/// 
	/// <em>number</em> - The position in the movie when the frame will be displayed.
	/// <br/>
	/// 
	/// <em>label</em> - An optional name assigned to a frame. The GotoFrame2 object
	/// can be used to move to a named frame when playing a movie or movie clip.<br/>
	/// 
	/// <em>definitions</em> - A list containing objects that define items for
	/// display in a movie. Definitions are sub-classes of the Definition class and
	/// define shapes, fonts, images and sounds that are displayed or played by the
	/// Flash Player.<br/>
	/// 
	/// <em>commands</em> - A list containing objects that define commands that
	/// affect the display list or the Flash Player directly.<br/>
	/// 
	/// <em>actions</em> - A list that define actions that are executed when a
	/// frame is displayed.
	/// </para>
	/// 
	/// <para>
	/// Frame objects simplify the handling of movies. DoAction, FrameLabel and
	/// ShowFrame classes can now "hidden" from view. They are generated
	/// automatically by the Frame object when it is added to an Movie object.
	/// </para>
	/// 
	/// <para>
	/// The framesFromMovie(Movie aMovie) method allows an existing movie to be
	/// viewed as a list of Frame objects. Objects from the movie are copied into
	/// each frame so changes made to the attributes of each object are reflected in
	/// the movie. The frame objects are not synchronised with the movie, so any
	/// objects added to a frame are not added to the Movie. The easiest way to do
	/// this is to remove the existing objects from the movie and add all the frames.
	/// </para>
	/// 
	/// <pre>
	/// ArrayList frames = Frame.framesFromMovie(aMovie);
	/// ...
	/// ...
	/// aMovie.getObjects().clear();
	/// 
	/// for (Iterator i = frames.iterator(); i.hasNext();) {
	///     ((Frame)i.next()).addToMovie(aMovie);
	/// }
	/// </pre>
	/// 
	/// <para>
	/// When the contents of an Frame object is added to a movie if a label defined
	/// then an FrameLabel object will be added. Similarly if actions are defined
	/// then an DoAction object will be added. An ShowFrame object which instructs
	/// the Flash Player to update the display list with all the changes is added.
	/// </para>
	/// 
	/// </summary>
	public sealed class Frame
	{
		/// <summary>
		/// Create a frame based view of a movie. Objects from the movie are grouped
		/// into Frame objects. Objects from the movie are added to the frame so any
		/// changes made are reflected in the movie. However objects added or removed
		/// from a frame are not reflected in the movie.
		/// </summary>
		/// <param name="aMovie">
		///            an Movie object. </param>
		/// <returns> a list of Frame objects. </returns>


		public static IList<Frame> split(Movie aMovie)
		{


			List<Frame> frames = new List<Frame>();
			int index = 1;
			Frame currentFrame = new Frame();

			foreach (MovieTag currentObject in aMovie.Objects)
			{
				if (currentObject is DoAction)
				{
					currentFrame.actions = ((DoAction) currentObject).Actions;
				}
				else if (currentObject is FrameLabel)
				{
					currentFrame.label = ((FrameLabel) currentObject).Label;
				}
				else if (currentObject is DefineTag)
				{
					currentFrame.addDefinition(currentObject);
				}
				else if (currentObject is ShowFrame)
				{
					currentFrame.Number = index++;
					frames.Add(currentFrame);
					currentFrame = new Frame();
				}
				else
				{
					currentFrame.addCommand(currentObject);
				}
			}
			return frames;
		}

		/// <summary>
		/// The frame label. </summary>
		private string label;
		/// <summary>
		/// The frame number. </summary>
		private int number;
		/// <summary>
		/// List of definitions. </summary>
		private IList<MovieTag> definitions;
		/// <summary>
		/// List of display list and other commands. </summary>
		private IList<MovieTag> commands;
		/// <summary>
		/// List of actions executed when the frame is displayed. </summary>
		private IList<Action> actions;

		/// <summary>
		/// Creates a empty frame with no label defined and the definitions, commands
		/// and actions lists empty.
		/// </summary>
		public Frame()
		{
			definitions = new List<MovieTag>();
			commands = new List<MovieTag>();
			actions = new List<Action>();
		}

		/// <summary>
		/// Creates a empty frame with no label defined and the definitions, commands
		/// and actions lists empty.
		/// </summary>
		/// <param name="frame"> the frame number. </param>


		public Frame(int frame)
		{
			Number = frame;
			definitions = new List<MovieTag>();
			commands = new List<MovieTag>();
			actions = new List<Action>();
		}

		/// <summary>
		/// Adds the action object to the frame.
		/// </summary>
		/// <param name="anObject">
		///            the action object to be added to the frame. Must not be null. </param>


		public void addAction(Action anObject)
		{
			if (anObject == null)
			{
				throw new ArgumentException();
			}
			actions.Add(anObject);
		}

		/// <summary>
		/// Adds an object to the frame that defines an object to be displayed in the
		/// movie.
		/// </summary>
		/// <param name="anObject">
		///            a sub-class of Definition. Must not be null. </param>


		public void addDefinition(MovieTag anObject)
		{
			if (anObject == null)
			{
				throw new ArgumentException();
			}
			definitions.Add(anObject);
		}

		/// <summary>
		/// Adds the display list command to the frame.
		/// </summary>
		/// <param name="anObject">
		///            an MovieTag the manipulates the display list. Must not be
		///            null. </param>


		public void addCommand(MovieTag anObject)
		{
			if (anObject == null)
			{
				throw new ArgumentException();
			}
			commands.Add(anObject);
		}

		/// <summary>
		/// Get the number of the frame.
		/// </summary>
		/// <returns> the frame number. </returns>
		public int Number
		{
			get => number;
		    set => number = value;
		}

		/// <summary>
		/// Returns the label assigned to the frame.
		/// </summary>
		/// <returns> the label. The string will be empty if no label is defined. </returns>
		public string Label
		{
			get => label;
		    set => label = value;
		}

		/// <summary>
		/// Returns the list of definition objects contained in the frame.
		/// </summary>
		/// <returns> the list of definitions. </returns>
		public IList<MovieTag> Definitions
		{
			get => definitions;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				definitions = value;
			}
		}

		/// <summary>
		/// Returns the list of commands that update the display list.
		/// </summary>
		/// <returns> the list of commands objects. </returns>
		public IList<MovieTag> Commands
		{
			get => commands;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				commands = value;
			}
		}

		/// <summary>
		/// Returns the array of action objects that will be execute when the frame
		/// is displayed.
		/// </summary>
		/// <returns> the array of actions defined for the frame. </returns>
		public IList<Action> Actions
		{
			get => actions;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				actions = value;
			}
		}






		/// <summary>
		/// Add the objects in the frame to the movie. The contents of the
		/// definitions and commands lists are added to the movie. If a label is
		/// assigned to the frame then an FrameLabel object is added to the movie. If
		/// actions are defined then an DoAction object is added containing the
		/// actions defined in the frame.
		/// </summary>
		/// <param name="aMovie">
		///            an Movie object. Must not be null. </param>


		public void addToMovie(Movie aMovie)
		{
			if (definitions.Count > 0)
			{
				foreach (MovieTag @object in definitions)
				{
					aMovie.add(@object);
				}
			}

			if (label.Length > 0)
			{
				aMovie.add(new FrameLabel(label));
			}

			if (actions.Count > 0)
			{
				aMovie.add(new DoAction(actions));
			}

			foreach (MovieTag @object in commands)
			{
				aMovie.add(@object);
			}

			aMovie.add(ShowFrame.Instance);
		}
	}

}