import { gql } from '@apollo/client';
import * as Apollo from '@apollo/client';
export type Maybe<T> = T | null;
export type Exact<T extends { [key: string]: unknown }> = {
  [K in keyof T]: T[K];
};
export type MakeOptional<T, K extends keyof T> = Omit<T, K> &
  { [SubKey in K]?: Maybe<T[SubKey]> };
export type MakeMaybe<T, K extends keyof T> = Omit<T, K> &
  { [SubKey in K]: Maybe<T[SubKey]> };
/** All built-in and custom scalars, mapped to their actual values */
export type Scalars = {
  ID: string;
  String: string;
  Boolean: boolean;
  Int: number;
  Float: number;
};

export type Compose = {
  __typename?: 'Compose';
  id: Scalars['ID'];
  deployments: Array<Deployment>;
  name: Scalars['String'];
  content: Scalars['String'];
  serviceEnabled: Scalars['Boolean'];
  serviceName?: Maybe<Scalars['String']>;
};

export type Deployment = {
  __typename?: 'Deployment';
  id: Scalars['ID'];
  node: Node;
  compose: Compose;
  enabled: Scalars['Boolean'];
};

export type Node = {
  __typename?: 'Node';
  id: Scalars['ID'];
  name: Scalars['String'];
  host: Scalars['String'];
  port: Scalars['Int'];
  username: Scalars['String'];
  deployments: Array<Deployment>;
};

export type Query = {
  __typename?: 'Query';
  compose: Compose;
  composes: Array<Compose>;
  node: Node;
  nodes: Array<Node>;
};

export type QueryComposeArgs = {
  id: Scalars['ID'];
};

export type QueryNodeArgs = {
  id: Scalars['ID'];
};

export type Mutation = {
  __typename?: 'Mutation';
  createNode: Node;
  testConnection: Scalars['Boolean'];
};

export type MutationCreateNodeArgs = {
  node: CreateNodeInput;
};

export type MutationTestConnectionArgs = {
  node: CreateNodeInput;
};

export type CreateNodeInput = {
  name: Scalars['String'];
  host: Scalars['String'];
  port: Scalars['Int'];
  username: Scalars['String'];
  password?: Maybe<Scalars['String']>;
  privateKey?: Maybe<Scalars['String']>;
};

export type TestConnectionMutationVariables = Exact<{
  name: Scalars['String'];
  host: Scalars['String'];
  port: Scalars['Int'];
  username: Scalars['String'];
  password?: Maybe<Scalars['String']>;
  privateKey?: Maybe<Scalars['String']>;
}>;

export type TestConnectionMutation = { __typename?: 'Mutation' } & Pick<
  Mutation,
  'testConnection'
>;

export type GetComposesQueryVariables = Exact<{ [key: string]: never }>;

export type GetComposesQuery = { __typename?: 'Query' } & {
  composes: Array<{ __typename?: 'Compose' } & Pick<Compose, 'id' | 'name'>>;
};

export type GetNodesQueryVariables = Exact<{ [key: string]: never }>;

export type GetNodesQuery = { __typename?: 'Query' } & {
  nodes: Array<
    { __typename?: 'Node' } & Pick<Node, 'id' | 'name' | 'host' | 'username'>
  >;
};

export const TestConnectionDocument = gql`
  mutation testConnection(
    $name: String!
    $host: String!
    $port: Int!
    $username: String!
    $password: String
    $privateKey: String
  ) {
    testConnection(
      node: {
        name: $name
        host: $host
        port: $port
        username: $username
        password: $password
        privateKey: $privateKey
      }
    )
  }
`;
export type TestConnectionMutationFn = Apollo.MutationFunction<
  TestConnectionMutation,
  TestConnectionMutationVariables
>;

/**
 * __useTestConnectionMutation__
 *
 * To run a mutation, you first call `useTestConnectionMutation` within a React component and pass it any options that fit your needs.
 * When your component renders, `useTestConnectionMutation` returns a tuple that includes:
 * - A mutate function that you can call at any time to execute the mutation
 * - An object with fields that represent the current status of the mutation's execution
 *
 * @param baseOptions options that will be passed into the mutation, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options-2;
 *
 * @example
 * const [testConnectionMutation, { data, loading, error }] = useTestConnectionMutation({
 *   variables: {
 *      name: // value for 'name'
 *      host: // value for 'host'
 *      port: // value for 'port'
 *      username: // value for 'username'
 *      password: // value for 'password'
 *      privateKey: // value for 'privateKey'
 *   },
 * });
 */
export function useTestConnectionMutation(
  baseOptions?: Apollo.MutationHookOptions<
    TestConnectionMutation,
    TestConnectionMutationVariables
  >,
) {
  return Apollo.useMutation<
    TestConnectionMutation,
    TestConnectionMutationVariables
  >(TestConnectionDocument, baseOptions);
}
export type TestConnectionMutationHookResult = ReturnType<
  typeof useTestConnectionMutation
>;
export type TestConnectionMutationResult = Apollo.MutationResult<TestConnectionMutation>;
export type TestConnectionMutationOptions = Apollo.BaseMutationOptions<
  TestConnectionMutation,
  TestConnectionMutationVariables
>;
export const GetComposesDocument = gql`
  query getComposes {
    composes {
      id
      name
    }
  }
`;

/**
 * __useGetComposesQuery__
 *
 * To run a query within a React component, call `useGetComposesQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetComposesQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetComposesQuery({
 *   variables: {
 *   },
 * });
 */
export function useGetComposesQuery(
  baseOptions?: Apollo.QueryHookOptions<
    GetComposesQuery,
    GetComposesQueryVariables
  >,
) {
  return Apollo.useQuery<GetComposesQuery, GetComposesQueryVariables>(
    GetComposesDocument,
    baseOptions,
  );
}
export function useGetComposesLazyQuery(
  baseOptions?: Apollo.LazyQueryHookOptions<
    GetComposesQuery,
    GetComposesQueryVariables
  >,
) {
  return Apollo.useLazyQuery<GetComposesQuery, GetComposesQueryVariables>(
    GetComposesDocument,
    baseOptions,
  );
}
export type GetComposesQueryHookResult = ReturnType<typeof useGetComposesQuery>;
export type GetComposesLazyQueryHookResult = ReturnType<
  typeof useGetComposesLazyQuery
>;
export type GetComposesQueryResult = Apollo.QueryResult<
  GetComposesQuery,
  GetComposesQueryVariables
>;
export const GetNodesDocument = gql`
  query getNodes {
    nodes {
      id
      name
      host
      username
    }
  }
`;

/**
 * __useGetNodesQuery__
 *
 * To run a query within a React component, call `useGetNodesQuery` and pass it any options that fit your needs.
 * When your component renders, `useGetNodesQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useGetNodesQuery({
 *   variables: {
 *   },
 * });
 */
export function useGetNodesQuery(
  baseOptions?: Apollo.QueryHookOptions<GetNodesQuery, GetNodesQueryVariables>,
) {
  return Apollo.useQuery<GetNodesQuery, GetNodesQueryVariables>(
    GetNodesDocument,
    baseOptions,
  );
}
export function useGetNodesLazyQuery(
  baseOptions?: Apollo.LazyQueryHookOptions<
    GetNodesQuery,
    GetNodesQueryVariables
  >,
) {
  return Apollo.useLazyQuery<GetNodesQuery, GetNodesQueryVariables>(
    GetNodesDocument,
    baseOptions,
  );
}
export type GetNodesQueryHookResult = ReturnType<typeof useGetNodesQuery>;
export type GetNodesLazyQueryHookResult = ReturnType<
  typeof useGetNodesLazyQuery
>;
export type GetNodesQueryResult = Apollo.QueryResult<
  GetNodesQuery,
  GetNodesQueryVariables
>;
