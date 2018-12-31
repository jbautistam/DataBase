using System;
using System.Linq;

using Bau.Libraries.LibCommonHelper.Extensors;
using Bau.Libraries.PivotTableExtended.Definitions;

namespace Bau.Libraries.PivotTableExtended.Results
{
	/// <summary>
	///		Colección de <see cref="LabelModel"/>
	/// </summary>
	public class LabelModelCollection : Base.BaseModelCollection<LabelModel>
	{
		public LabelModelCollection(GroupModel objGroup)
		{ Group = objGroup;
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
					strDebug += "Grupo: " + Group.Title + " - " + Group.Type.ToString() + Environment.NewLine;
					foreach (LabelModel objLabel in this)
						strDebug += objLabel.GetDebugStructure(intLevel + 1);
				// Devuelve la cadena de depuración
					return strDebug;
		}

		/// <summary>
		///		Añade u obtiene la etiqueta de un grupo
		/// </summary>
		internal LabelModel Add(LabelModel objParent, string strTitle)
		{ return Add(objParent, strTitle, false);
		}

		/// <summary>
		///		Añade u obtiene una etiqueta de total
		/// </summary>
		internal LabelModel AddTotal(GroupModel objGroupTotal, LabelModel objParent, string strTitle)
		{ LabelModel objLabel = Add(objParent, strTitle, true);

				// Devuelve la etiqueta
					return objLabel;
		}

		/// <summary>
		///		Ordena por título
		/// </summary>
		internal void SortByTitle()
		{ // Ordena las etiquetas
				this.Sort((objFirst, objSecond) =>
											{ if (objFirst.IsTotal)
													return 1;
												else if (objSecond.IsTotal)
													return -1;
												else
													return objFirst.Title.CompareTo(objSecond.Title);
											}
								  );
			// Ordena los hijos
				foreach (LabelModel objLabel in this)
					if (objLabel.Childs != null)
						objLabel.Childs.SortByTitle();
		}

		/// <summary>
		///		Añade u obtiene una etiqueta
		/// </summary>
		private LabelModel Add(LabelModel objParent, string strTitle, bool blnIsTotal)
		{ LabelModel objLabel;
		
				// Normaliza la etiqueta
					strTitle = strTitle.TrimIgnoreNull();
				// Busca la etiqueta adecuada
					if (blnIsTotal)
						objLabel = SearchTotal();
					else
						objLabel = SearchByTitle(strTitle);
				// Si no ha encontrado la etiqueta la añade
					if (objLabel == null)
						{ // Crea la etiqueta
								objLabel = new LabelModel(this, objParent, strTitle, blnIsTotal);
							// La añade a la colección
								Add(objLabel);
						}
				// Devuelve la etiqueta
					return objLabel;
		}

		/// <summary>
		///		Calcula el span de fila o column
		/// </summary>
		internal int ComputeSpan()
		{ int intSpan = 0;

				// Acumula el span de las etiquetas hija
					foreach (LabelModel objLabel in this)
						intSpan += objLabel.ComputeSpan();
				// Devuelve el span
					return intSpan;
		}

		/// <summary>
		///		Busca una etiqueta por su título
		/// </summary>
		public LabelModel SearchByTitle(string strTitle)
		{ return this.FirstOrDefault<LabelModel>(objLabel => objLabel.Title.EqualsIgnoreCase(strTitle));
		}

		/// <summary>
		///		Busca la etiqueta de total
		/// </summary>
		public LabelModel SearchTotal()
		{ return this.FirstOrDefault<LabelModel>(objLabel => objLabel.IsTotal);
		}

		/// <summary>
		///		Grupo al que pertenece esta colección
		/// </summary>
		public GroupModel Group { get; private set; }
	}
}
