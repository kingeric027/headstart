using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ordercloud.integrations.cms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Tests
{
	public class SchemaTests
	{
		private DocSchemaDO GetSchema(string json) => new DocSchemaDO() { Schema = JObject.Parse(json) };
		private DocumentDO GetDocument(string json) => new DocumentDO() { Doc = JObject.Parse(json) };

		private const string ValidSchema = @"{ 
										'type': 'object',
										'properties': {
											'name': {'type': 'string'},
											'roles': {'type': 'array'}
										},
										'required': ['name'],
										'additionalProperties': false,
									}";


		[Test, TestCase(@"{
                'name': 'Spider Man',
				'roles': ['superhero', 'chnid']
				}")]
		public async Task valid_documents_produce_no_errors(string document)
		{
			Assert.DoesNotThrowAsync(async () =>
				await SchemaHelper.ValidateDocumentAgainstSchema(GetSchema(ValidSchema), GetDocument(document))
			);
		}

		[Test, TestCase(@"{
                'name': false,
				'roles': ['superhero', 'arachnid']
				}"),
				TestCase(@"{ 'roles': ['superhero', 'arachnid'] }"),
				TestCase(@"{
                'name': 'Spider Man',
				'roles': ['superhero', 'arachnid'],
				'brand-equity': 1000
				}")]
		public async Task invalid_document_errors(string document)
		{
			Assert.ThrowsAsync<DocumentNotValidException>(async () => 
				await SchemaHelper.ValidateDocumentAgainstSchema(GetSchema(ValidSchema), GetDocument(document))
			);
		}

		[Test, TestCase(ValidSchema)]
		public async Task valid_schemas_produce_no_errors(string schema)
		{
			Assert.DoesNotThrowAsync(async () =>
				await SchemaHelper.ValidateSchema(GetSchema(schema))
			);
		}

		[Test, TestCase(@"{ 
					'type': 'object',
					'properties': {
						'name': {'type': 'InvalidType'},
						'roles': {'type': 'array'}
					},
					'required': ['name']
				}")]
		public async Task invalid_schema_errors(string schema)
		{
			Assert.ThrowsAsync<SchemaNotValidException>(async () =>
				await SchemaHelper.ValidateSchema(GetSchema(schema))
			);
		}
	}
}
