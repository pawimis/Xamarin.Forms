using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Forms.Xaml;
using Xamarin.Forms.Exceptions;

using static Mono.Cecil.Cil.Instruction;
using static Mono.Cecil.Cil.OpCodes;

namespace Xamarin.Forms.Build.Tasks
{
	class TypeExtension : ICompiledMarkupExtension
	{
		public IEnumerable<Instruction> ProvideValue(IElementNode node, ModuleDefinition module, ILContext context, out TypeReference memberRef)
		{
			memberRef = module.ImportReference(("mscorlib", "System", "Type"));
			var name = new XmlName("", "TypeName");

			if (!node.Properties.TryGetValue(name, out INode typeNameNode) && node.CollectionItems.Any())
				typeNameNode = node.CollectionItems[0];

			if (!(typeNameNode is ValueNode valueNode))
				throw new XamlParseException("XF0032", node as XmlLineInfo);

			if (!node.Properties.ContainsKey(name)) {
				node.Properties[name] = typeNameNode;
				node.CollectionItems.Clear();
			}

			var typeref = module.ImportReference(XmlTypeExtensions.GetTypeReference(valueNode.Value as string, module, node as BaseNode));

			context.TypeExtensions[node] = typeref ?? throw new XamlParseException("XF0001", node as IXmlLineInfo, valueNode.Value.ToString());

			return new List<Instruction> {
				Create(Ldtoken, module.ImportReference(typeref)),
				Create(Call, module.ImportMethodReference(("mscorlib", "System", "Type"),
														  methodName: "GetTypeFromHandle",
														  parameterTypes: new[] { ("mscorlib", "System", "RuntimeTypeHandle") },
														  isStatic: true)),
			};
		}
	}
}