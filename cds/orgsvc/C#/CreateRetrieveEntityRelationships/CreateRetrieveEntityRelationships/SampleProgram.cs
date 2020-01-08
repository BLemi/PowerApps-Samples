﻿using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Required to support the interactive login experience
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {
                    // Create any entity records that the demonstration code requires
                    SetUpSample(service);
                    #region Demonstrate
                    bool eligibleCreateOneToManyRelationship = EligibleCreateOneToManyRelationship(service, "account", "campaign");

                    if (eligibleCreateOneToManyRelationship)
                    {
                        var createOneToManyRelationshipRequest =
                            new CreateOneToManyRequest
                            {
                                OneToManyRelationship =
                            new OneToManyRelationshipMetadata
                            {
                                ReferencedEntity = "account",
                                ReferencingEntity = "campaign",
                                SchemaName = "new_account_campaign",
                                AssociatedMenuConfiguration = new AssociatedMenuConfiguration
                                {
                                    Behavior = AssociatedMenuBehavior.UseLabel,
                                    Group = AssociatedMenuGroup.Details,
                                    Label = new Label("Account", 1033),
                                    Order = 10000
                                },
                                CascadeConfiguration = new CascadeConfiguration
                                {
                                    Assign = CascadeType.NoCascade,
                                    Delete = CascadeType.RemoveLink,
                                    Merge = CascadeType.NoCascade,
                                    Reparent = CascadeType.NoCascade,
                                    Share = CascadeType.NoCascade,
                                    Unshare = CascadeType.NoCascade
                                }
                            },
                                Lookup = new LookupAttributeMetadata
                                {
                                    SchemaName = "new_parent_accountid",
                                    DisplayName = new Label("Account Lookup", 1033),
                                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                                    Description = new Label("Sample Lookup", 1033)
                                }
                            };


                        var createOneToManyRelationshipResponse = (CreateOneToManyResponse)service.Execute(
                            createOneToManyRelationshipRequest);

                        _oneToManyRelationshipId =
                            createOneToManyRelationshipResponse.RelationshipId;
                        _oneToManyRelationshipName =
                            createOneToManyRelationshipRequest.OneToManyRelationship.SchemaName;

                        Console.WriteLine(
                            "The one-to-many relationship has been created between {0} and {1}.",
                            "account", "campaign");
                    }

                    

                    bool accountEligibleParticipate =
                        EligibleCreateManyToManyRelationship(service, "account");
                    bool campaignEligibleParticipate =
                        EligibleCreateManyToManyRelationship(service, "campaign");

                    if (accountEligibleParticipate && campaignEligibleParticipate)
                    {

                        var createManyToManyRelationshipRequest =
                            new CreateManyToManyRequest
                            {
                                IntersectEntitySchemaName = "new_accounts_campaigns",
                                ManyToManyRelationship = new ManyToManyRelationshipMetadata
                                {
                                    SchemaName = "new_accounts_campaigns",
                                    Entity1LogicalName = "account",
                                    Entity1AssociatedMenuConfiguration =
                                new AssociatedMenuConfiguration
                                {
                                    Behavior = AssociatedMenuBehavior.UseLabel,
                                    Group = AssociatedMenuGroup.Details,
                                    Label = new Label("Account", 1033),
                                    Order = 10000
                                },
                                    Entity2LogicalName = "campaign",
                                    Entity2AssociatedMenuConfiguration =
                                new AssociatedMenuConfiguration
                                {
                                    Behavior = AssociatedMenuBehavior.UseLabel,
                                    Group = AssociatedMenuGroup.Details,
                                    Label = new Label("Campaign", 1033),
                                    Order = 10000
                                }
                                }
                            };

                        var createManytoManyRelationshipResponse =
                            (CreateManyToManyResponse)service.Execute(
                            createManyToManyRelationshipRequest);


                        _manyToManyRelationshipId =
                            createManytoManyRelationshipResponse.ManyToManyRelationshipId;
                        _manyToManyRelationshipName =
                            createManyToManyRelationshipRequest.ManyToManyRelationship.SchemaName;

                        Console.WriteLine(
                            "The many-to-many relationship has been created between {0} and {1}.",
                            "account", "campaign");
                    }

                    

                    // Publish the customization changes.
                    service.Execute(new PublishAllXmlRequest());

                    //You can use either the Name or the MetadataId of the relationship.

                    //Retrieve the One-to-many relationship using the MetadataId.
                    var retrieveOneToManyRequest = new RetrieveRelationshipRequest { MetadataId = _oneToManyRelationshipId };
                    var retrieveOneToManyResponse = (RetrieveRelationshipResponse)service.Execute(retrieveOneToManyRequest);

                    Console.WriteLine("Retrieved {0} One-to-many relationship by id", retrieveOneToManyResponse.RelationshipMetadata.SchemaName);

                    //Retrieve the Many-to-many relationship using the Name.
                    var retrieveManyToManyRequest = new RetrieveRelationshipRequest { Name = _manyToManyRelationshipName };
                    var retrieveManyToManyResponse = (RetrieveRelationshipResponse)service.Execute(retrieveManyToManyRequest);

                    Console.WriteLine("Retrieved {0} Many-to-Many relationship by Name", retrieveManyToManyResponse.RelationshipMetadata.MetadataId);
                    #endregion Demonstrate

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Common Data Service";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw service.LastCrmException;
                    }
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.HandleException(ex);
            }

            finally
            {
                if (service != null)
                    service.Dispose();

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }

        }
    }
}
