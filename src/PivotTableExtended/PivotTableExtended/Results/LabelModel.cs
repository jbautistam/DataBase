using System;

using Bau.Libraries.LibCommonHelper.Extensors;

namespace Bau.Libraries.PivotTableExtended.Results
{
	/// <summary>
	///		Etiqueta
	/// </summary>
	public class LabelModel : Base.BaseModel
	{
		public LabelModel(LabelModelCollection objColLabels, LabelModel objParent, string strTitle, bool blnIsTotal, Definitions.GroupModel objGroup = null)
		{ RowLabels = objColLabels;
			Parent = objParent;
			Group = objGroup;
			Title = strTitle;
			IsTotal = blnIsTotal;
		}

		/// <summary>
		///		Comprueba si una etiqueta es igual a otra
		/// </summary>
		internal bool IsEqual(LabelModel objLabel)
		{ return GlobalID.EqualsIgnoreCase(objLabel.GlobalID);
		}

		/// <summary>
		///		Obtiene el ID completo de la etiqueta (el suyo más el de todos sus elementos padre)
		/// </summary>
		internal string GetFullID()
		{ string strID = base.GlobalID;

				// Añade el ID del padre
				if (Parent != null)
					strID += "@@" + Parent.GetFullID();
				// Devuelve el ID
					return strID;
		}

		/// <summary>
		///		Obtiene el título completo de una etiqueta
		/// </summary>
		internal string GetFullTitle()
		{ string strTitle = "";

				// Asigna el título del padre
					if (Parent != null)
						strTitle = Parent.GetFullTitle() + " --> ";
				// Devuelve el título completo
					return strTitle + Title;
		}

		/// <summary>
		///		Calcula el span
		/// </summary>
		internal int ComputeSpan()
		{ if (Childs == null)
				return 1;
			else
				return Childs.ComputeSpan();
		}

		/// <summary>
		///		Obtiene la estructura de depuración
		/// </summary>
		internal string GetDebugStructure(int intLevel = 0)
		{ string strDebug = "";

				// Añade la indentación
					for (int intIndex = 0; intIndex < intLevel; intIndex++)
						strDebug += "\t";
				// Añade la información
					strDebug += "Etiqueta: " + Title;
					if (Parent != null)
						strDebug += " --> Padre: " + Parent.Title;
				// Añade la información de las etiquetas que se totalizan
					if (LabelsTotal != null)
						{ strDebug += " (Total de: ";
							foreach (LabelModel objLabel in LabelsTotal)
								strDebug += objLabel.GetFullTitle() + " /// ";
							strDebug += ")";
						}
				// Salto de línea
					strDebug += Environment.NewLine;
				// Añade la información de los hijos
					if (Childs != null)
						strDebug += Childs.GetDebugStructure(intLevel + 1);
				// Devuelve la cadena de depuración
					return strDebug;
		}

		/// <summary>
		///		Colección de etiquetas a la que pertenece ésta
		/// </summary>
		public LabelModelCollection RowLabels { get; private set; }

		/// <summary>
		///		Etiqueta padre
		/// </summary>
		public LabelModel Parent { get; private set; }

		/// <summary>
		///		Título de la etiqueta
		/// </summary>
		public string Title { get; private set; }

		/// <summary>
		///		Grupo al que se asocia la etiqueta (sobre todo para los grupos de celda finales)
		/// </summary>
		public Definitions.GroupModel Group { get; internal set; }

		/// <summary>
		///		Etiquetas que se totalizan
		/// </summary>
		public LabelModelCollection LabelsTotal { get; internal set; }

		/// <summary>
		///		Indica si es una celda de total
		/// </summary>
		public bool IsTotal { get; set; }

		/// <summary>
		///		Etiquetas hija
		/// </summary>
		public LabelModelCollection Childs { get; set; }
	}
}
