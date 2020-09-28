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
			Assert.DoesNotThrow(() =>
				SchemaHelper.ValidateDocumentAgainstSchema(GetSchema(ValidSchema), GetDocument(document))
			);
		}

		[Test, TestCase(@"{
                'name': 'Spider Man',
				'roles': ['superhero', 'arachnid'],
				'brand-equity': 1000
				}")]
		public async Task document_with_additionalProperties_errors(string document)
		{
			Assert.Throws<DocumentNotValidException>(() =>
				SchemaHelper.ValidateDocumentAgainstSchema(GetSchema(ValidSchema), GetDocument(document))
			);
		}

		[Test, TestCase(@"{
				'roles': ['superhero', 'arachnid'],
				}")]
		public async Task document_missing_required_field_errors(string document)
		{
			Assert.Throws<DocumentNotValidException>(() =>
				SchemaHelper.ValidateDocumentAgainstSchema(GetSchema(ValidSchema), GetDocument(document))
			);
		}

		[Test, TestCase(@"{
                'name': false,
				'roles': ['superhero', 'arachnid']
				}")]
		public async Task document_with_wrong_type_errors(string document)
		{
			Assert.Throws<DocumentNotValidException>(() => 
				SchemaHelper.ValidateDocumentAgainstSchema(GetSchema(ValidSchema), GetDocument(document))
			);
		}

		[Test, TestCase(ValidSchema)]
		public async Task valid_schemas_produce_no_errors(string schema)
		{
			Assert.DoesNotThrow(() =>
				SchemaHelper.ValidateSchema(GetSchema(schema))
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
		public async Task schema_with_invalid_type_errors(string schema)
		{
			Assert.Throws<SchemaNotValidException>(() =>
				SchemaHelper.ValidateSchema(GetSchema(schema))
			);
		}

		[Test, TestCase(@"{ 
							'type': 'object',
							'properties': {
								'name': {'type': 'string'},
								'roles': {'type': 'array'}
							},
							'required': ['name'],
							'Extraneous': 'field'
						}")]
		public async Task schema_with_extra_field_errors(string schema)
		{
			Assert.Throws<SchemaNotValidException>(() =>
				SchemaHelper.ValidateSchema(GetSchema(schema))
			);
		}

		[Test, TestCase(@"{ 
							'type': 'object',
							'propertiez': {
								'name': {'type': 'string'},
								'roles': {'type': 'array'}
							},
							'required': ['name']
						}")]
		public async Task schema_with_misspelling_errors(string schema)
		{
			Assert.Throws<SchemaNotValidException>(() =>
				SchemaHelper.ValidateSchema(GetSchema(schema))
			);
		}
	}
}
