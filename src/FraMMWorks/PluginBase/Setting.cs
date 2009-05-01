/*
    FraMMWorks - Multimedia Processing and Diagnostic Framework.
    Copyright (C) 2008  Wilson Waters

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FraMMWorks.PluginBase
{
   /// <summary>
   /// Holds information about modifyable settings provided by each plugin.
   /// The plugin exposes each setting in an instance of this class and the
   /// GUI drawing classes will directly modify the value as it is modified
   /// by the user.
   /// NOTE: this type must be a class, not a struct. classes are stored as
   /// references, as opposed to structs which are copied.
   /// 
   /// The following types of primitive settings are recognised by the GUI:
   /// Int32
   /// Double
   /// String
   /// Settings with the Type field as one of these will have controls
   /// automatically drawn and values updated. For more complex controls
   /// (such as a video frame picker) extend the Control class
   /// and implement the control drawing methods. Ths will cause your
   /// custom control to be drawn to the settings screen and update however
   /// you design it.
   /// </summary>
   public class Setting
   {
      /// <summary>
      /// The displayable name of this setting;
      /// </summary>
      public String name;

      /// <summary>
      /// The Type of this control (i.e. int, string, CustomControl)
      /// </summary>
      public Type type;

      /// <summary>
      /// a minimum value for this control
      /// </summary>
      public double minValue;

      /// <summary>
      /// a maximum value for this control
      /// </summary>
      public double maxValue;

      /// <summary>
      /// The current value of this control. the Type of this value is defined
      /// in the Type field. This may also be a CustomControl object.
      /// </summary>
      public Object value;

      /// <summary>
      /// flag to indicate this setting has changed (and nothing had been done about it)
      /// </summary>
      public bool isDirty;

   }
}
