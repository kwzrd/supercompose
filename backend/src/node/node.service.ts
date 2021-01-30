import { Injectable } from '@nestjs/common';
import { InjectEntityManager, InjectRepository } from '@nestjs/typeorm';
import { EntityManager, Repository } from 'typeorm';
import { NodeEntity } from './node.entity';
import { NodeVersionEntity } from './nodeVersion.entity';
import { v4 as uuid } from 'uuid';
import { CryptoService } from 'src/crypto/crypto.service';

@Injectable()
export class NodeService {
  constructor(
    @InjectRepository(NodeEntity)
    private readonly nodeRepo: Repository<NodeEntity>,
    @InjectEntityManager()
    private readonly manager: EntityManager,
    private readonly crypto: CryptoService,
  ) {}

  public async createNode(args: {
    name: string;
    host: string;
    username: string;
    port: number;
    password: string;
    privateKey: string;
  }) {
    const nodeConfigEntity = new NodeVersionEntity();
    nodeConfigEntity.id = uuid();
    nodeConfigEntity.composes = [];

    const nodeEntity = new NodeEntity();
    nodeEntity.id = uuid();
    nodeEntity.name = args.name;
    nodeEntity.target = nodeConfigEntity;
    nodeEntity.host = args.host;
    nodeEntity.username = args.username;
    nodeEntity.port = args.port;
    nodeEntity.password = args.password
      ? await this.crypto.encryptSecret(args.password)
      : undefined;
    nodeEntity.privateKey = args.privateKey
      ? await this.crypto.encryptSecret(args.privateKey)
      : undefined;

    await this.manager.transaction(async trx => {
      await trx.save(nodeConfigEntity);
      await trx.save(nodeEntity);
    });

    return nodeEntity.id;
  }
}