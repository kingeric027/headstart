using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ordercloud.integrations.cms;
using ordercloud.integrations.cms.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Tests
{
	public class SchemaTests
	{
		private DocumentSchema GetSchema(string json) => new DocumentSchema() { Schema = JObject.Parse(json), Title = "A Title" };
		private CMSConfig GetConfig() => new CMSConfig() { BaseUrl = "http://fake.com" };

		[Test, TestCase(@"{ 
							'type': 'object',
							'properties': {
								'name': {'type': 'string'},
								'roles': {'type': 'array'}
							},
							'required': ['name']
						}")]
		public async Task valid_schemas_produce_no_errors(string schema)
		{
			Assert.DoesNotThrow(() =>
				SchemaHelper.ValidateSchema(GetSchema(schema), GetConfig())
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
				SchemaHelper.ValidateSchema(GetSchema(schema), GetConfig())
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
				SchemaHelper.ValidateSchema(GetSchema(schema), GetConfig())
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
				SchemaHelper.ValidateSchema(GetSchema(schema), GetConfig())
			);
		}
	}
}
